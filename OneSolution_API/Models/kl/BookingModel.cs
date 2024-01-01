using Newtonsoft.Json.Linq;
using OneSolution_API.Managers;
using OneSolution_API.Models.Login;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.kl
{
    public class BookingModel
    {
        public String id { get; set; }
        public String patientId { get; set; }
        public String scheduleId { get; set; }
        public String healthstatus { get; set; }
        public String nhommau { get; set; }
        public String isTaiKham { get; set; }
        public String code { get; set; }
        public String stt { get; set; }
		public String emailPatient { get; set; }
		public String namePatient { get; set; }
		public String dob { get; set; }
		public String phonenumber { get; set; }
		public String address { get; set; }
		public String age { get; set; }
		public ObjectResponse createBookingByPatient(BookingModel booking,String userId)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select * from Esche where pk_seq='{booking.scheduleId}'
								";
					String id = "";
					String name = "";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Không có lịch khám bệnh này, vui lòng thử lại:",
							content = booking
						};
					}
					if (String.IsNullOrEmpty(booking.patientId))
					{
						booking.patientId = userId;
					}
					query = $@"select * from Euser where (pk_seq='{booking.patientId}' or pk_seq='{userId}') and roleId=1
								";
                    
					dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Không có id bệnh nhân này, vui lòng thử lại:",
							content = booking
						};
					}
					query = $@"select top 1 isnull(c.ma,'') as maClinic,isnull(b.clinicid,0) as id
										,CONVERT(VARCHAR, CONVERT(DATE, a.date, 23), 103)as date,a.timedt as timedt,c.name 
										,isnull(b.price,0) price,a.doctorid as doctorId
									from Esche a
									left join Edt_info b on a.doctorid=b.userid
									left join Ecli_info c on b.clinicid=c.pk_seq
									where a.pk_seq='{booking.scheduleId}'
								";
					dt = db.getDataTable(query);
					String account = XuLy.ParseDataRowToJson(dt.Rows[0]);
					ClinicModel guest = JObject.Parse(account).ToObject<ClinicModel>();
					ScheduleModel schedule = JObject.Parse(account).ToObject<ScheduleModel>();
					DoctorModel dtm = JObject.Parse(account).ToObject<DoctorModel>();
					query = $@"select name,email from Euser where pk_seq='{booking.patientId}'
								";
					dt = db.getDataTable(query);
					account = XuLy.ParseDataRowToJson(dt.Rows[0]);
					userModel patient = JObject.Parse(account).ToObject<userModel>();

					query = $@"select isnull(max(stt),0) as stt from ma_sche where clinicid='{guest.id}'";
					dt = db.getDataTable(query);
					int stt = db.getFirsIntValueSqlCatchException(query)+1;
					String code = guest.maClinic + stt.ToString();
					db.BeginTran();
					query = $@"insert into ma_sche select '{guest.id}',{stt}";
                    if (!db.updateQuery(query))
                    {
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo booking",
							content = booking
						};
					}
					query = $@"select isnull(max(stt),0) as stt from Ebooking where scheduleId='{booking.scheduleId}'";
					dt = db.getDataTable(query);
					stt = db.getFirsIntValueSqlCatchException(query) + 1;

					query = $@"INSERT INTO [dbo].[Ebooking]
									   ([code]
									   ,[stt]
									   ,[scheduleId]
									   ,[patientId]
									   ,[statusId]
									   ,[createdAt]
									   ,[price],clinicId,doctorId
									   )
								select N'{code}',{stt},'{booking.scheduleId}','{booking.patientId}',1,GETDATE(),{dtm.price},'{guest.id}','{dtm.doctorId}'";

					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo booking",
							content = booking
						};
					}

					query = "select scope_identity()  as tthdId";
					String bookingId = "";
					DataTable rsTthd = db.getDataTable(query);
					foreach (DataRow row in rsTthd.Rows)
					{
						bookingId = row["tthdId"].ToString();
						break;
					}
					String ageText = "0";
                    if (!String.IsNullOrEmpty(booking.age))
                    {
						ageText = booking.age;
                    }

					query = $@" insert into bkinfo(bookingid,	bloodgr,	health_status,	taikham,patientName,age)
								select '{bookingId}','{booking.nhommau}',N'{booking.healthstatus}','{booking.isTaiKham}',N'{booking.namePatient}','{ageText}'
						";

					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo booking",
							content = booking
						};
					}
					if (!MailModel.sendMailtoPatient(patient, schedule, guest,code,stt,booking.namePatient))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi trong quá trình gửi mail",
							content = booking
						};
					}
					booking.code = code;
					db.CommitAndDispose();
					return new ObjectResponse
					{
						result = 1,
						message = $@"Success",
						content = booking
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}
		}
		public ObjectResponse createBookingByAdmin(BookingModel booking,String userId)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"select top 1 pk_seq from Euser where email='{booking.emailPatient}'
								";
					int isPatient = db.getFirsIntValueSqlCatchException(query);
					DataTable dt = new DataTable();
					String ageText = "0";
					if (!String.IsNullOrEmpty(booking.age))
					{
						ageText = booking.age;
					}
					if (isPatient <= 0)
                    {
						db.BeginTran();
						query = $@" insert into Euser(name,ngaysinh,email,username,password,trangthai,roleID,phonenumber,createdAt,address)
                                select N'{booking.namePatient}','{booking.dob}','{booking.emailPatient}','{booking.emailPatient}',PWDENCRYPT('123456'),1,1,'{booking.phonenumber}',GETDATE(),'{booking.address}'
                            ";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}
						query = "select scope_identity()  as tthdId";
						String usId = "";
						DataTable rs = db.getDataTable(query);
						foreach (DataRow row in rs.Rows)
						{
							usId = row["tthdId"].ToString();
							break;
						}
						query = $@"select top 1 isnull(c.ma,'') as maClinic,isnull(b.clinicid,0) as id
									,CONVERT(VARCHAR, CONVERT(DATE, a.date, 23), 103)as date,a.timedt as timedt,c.name 
									, isnull(b.price,0) as price,a.doctorid as doctorId
									from Esche a
									left join Edt_info b on a.doctorid=b.userid
									left join Ecli_info c on b.clinicid=c.pk_seq
									where a.pk_seq='{booking.scheduleId}'
								";
						dt = db.getDataTable(query);
						String account = XuLy.ParseDataRowToJson(dt.Rows[0]);
						ClinicModel guest = JObject.Parse(account).ToObject<ClinicModel>();
						ScheduleModel schedule = JObject.Parse(account).ToObject<ScheduleModel>();
						DoctorModel dtm = JObject.Parse(account).ToObject<DoctorModel>();

						query = $@"select isnull(max(stt),0) as stt from ma_sche where clinicid='{guest.id}'";
						dt = db.getDataTable(query);
						int stt = db.getFirsIntValueSqlCatchException(query) + 1;
						String code = guest.maClinic + stt.ToString();
						
						
						
						query = $@"insert into ma_sche select '{guest.id}',{stt}";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}
						query = $@"select isnull(max(stt),0) as stt from Ebooking where scheduleId='{booking.scheduleId}'";
						dt = db.getDataTable(query);
						stt = db.getFirsIntValueSqlCatchException(query) + 1;

						query = $@"INSERT INTO [dbo].[Ebooking]
									   ([code]
									   ,[stt]
									   ,[scheduleId]
									   ,[patientId]
									   ,[statusId]
									   ,[createdAt]
									   ,[price],clinicId,doctorId
									   )
								select N'{code}',{stt},'{booking.scheduleId}','{usId}',2,GETDATE(),{dtm.price},'{guest.id}','{dtm.doctorId}'";

						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}

						query = "select scope_identity()  as tthdId";
						String bookingId = "";
						DataTable rsTthd = db.getDataTable(query);
						foreach (DataRow row in rsTthd.Rows)
						{
							bookingId = row["tthdId"].ToString();
							break;
						}

						query = $@" insert into bkinfo(bookingid,	bloodgr,	health_status,	taikham,patientName,age)
								select '{bookingId}','{booking.nhommau}',N'{booking.healthstatus}','{booking.isTaiKham}',N'{booking.namePatient}','{ageText}'
						";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}
						query = $@"insert into reciept(bookingid,historyid,revenue,createdAt)
							select pk_seq,null,isnull(price,0),GETDATE() from Ebooking where pk_seq='{bookingId}'";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}
						MailModel.sendMailtoPatientByAdmin(booking.emailPatient, booking.namePatient, schedule, guest, code, stt);
						MailModel.sendMailbyAdmin(booking.emailPatient, booking.namePatient, "123456");
						
						db.CommitAndDispose();
						booking.code = code;
						return new ObjectResponse
						{
							result = 1,
							message = $@"Success",
							content = booking
						};
                    }
                    else
                    {
						query = $@"select top 1 isnull(c.ma,'') as maClinic,isnull(b.clinicid,0) as id,CONVERT(VARCHAR, CONVERT(DATE, a.date, 23), 103)as date,a.timedt as timedt,c.name ,isnull(b.price,0) price,a.doctorid as doctorId
									from Esche a
									left join Edt_info b on a.doctorid=b.userid
									left join Ecli_info c on b.clinicid=c.pk_seq
									where a.pk_seq='{booking.scheduleId}'
								";
						dt = db.getDataTable(query);
						String account = XuLy.ParseDataRowToJson(dt.Rows[0]);
						ClinicModel guest = JObject.Parse(account).ToObject<ClinicModel>();
						ScheduleModel schedule = JObject.Parse(account).ToObject<ScheduleModel>();
						DoctorModel dtm = JObject.Parse(account).ToObject<DoctorModel>();

						query = $@"select isnull(max(stt),0) as stt from ma_sche where clinicid='{guest.id}'";
						dt = db.getDataTable(query);
						int stt = db.getFirsIntValueSqlCatchException(query) + 1;
						String code = guest.maClinic + stt.ToString();

						db.BeginTran();

						query = $@"insert into ma_sche select '{guest.id}',{stt}";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}
						query = $@"select isnull(max(stt),0) as stt from Ebooking where scheduleId='{booking.scheduleId}'";
						dt = db.getDataTable(query);
						stt = db.getFirsIntValueSqlCatchException(query) + 1;

						query = $@"INSERT INTO [dbo].[Ebooking]
									   ([code]
									   ,[stt]
									   ,[scheduleId]
									   ,[patientId]
									   ,[statusId]
									   ,[createdAt]
									   ,[price],clinicId,doctorId
									   )
								select N'{code}',{stt},'{booking.scheduleId}','{isPatient}',2,GETDATE(),{dtm.price},'{guest.id}','{dtm.doctorId}'";

						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}

						query = "select scope_identity()  as tthdId";
						String bookingId = "";
						DataTable rsTthd = db.getDataTable(query);
						foreach (DataRow row in rsTthd.Rows)
						{
							bookingId = row["tthdId"].ToString();
							break;
						}

						query = $@" insert into bkinfo(bookingid,	bloodgr,	health_status,	taikham,patientName,age)
								select '{bookingId}','{booking.nhommau}',N'{booking.healthstatus}','{booking.isTaiKham}',N'{booking.namePatient}','{ageText}'
						";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}
						query = $@"insert into reciept(bookingid,historyid,revenue,createdAt)
							select pk_seq,null,isnull(price,0),GETDATE() from Ebooking where pk_seq='{bookingId}'";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi tạo booking",
								content = booking
							};
						}
						MailModel.sendMailtoPatientByAdmin(booking.emailPatient, booking.namePatient, schedule, guest, code, stt);
						booking.code = code;
						booking.id = bookingId;
						db.CommitAndDispose();
						return new ObjectResponse
						{
							result = 1,
							message = $@"Success",
							content = booking
						};
					}
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}
		}
		public ObjectResponse getListBookingNeedToPay(String userId,String keyword)
        {
            try
            {
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic,isnull(c.name,'') name  from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									left join Ecli_info c on c.pk_seq=isnull(a.clinicId,b.clinicid)
									where a.roleID in (2,3) and a.pk_seq='{userId}' and isnull(a.clinicId,b.clinicid) is not null
		
								";
					String id = "";
					String name = "";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}
					foreach (DataRow row in dt.Rows)
					{
						id = row["clinic"].ToString();
						name = row["name"].ToString();
					}

					query = $@"select bk.pk_seq as bookingId,bk.code as bookingCode, bk.stt as bookingStt
									, dt.name as DoctorName,u.name as PatientName,s.date , s.timedt as buoi , inf.bloodgr nhommau
									,inf.health_status,inf.taikham as isTaikham,inf.age, bk.price , u.email as emailPatient
									,sch.pk_seq as scheduleId
									,bk.statusId 									
									,case when bk.statusId=1 then N'Đã xác nhận' when bk.statusId=2 then N'Đã thanh toán' when bk.statusId =4 then N'Đã khám' else '' end as trangthai
									,bk.createdAt
									from Ebooking bk
									left join Esche s on bk.scheduleId=s.pk_seq
									left join Edt_info dti on s.doctorid=dti.userid
									left join Euser dt on s.doctorid=dt.pk_seq
									left join Euser u on bk.patientId=u.pk_seq
									left join bkinfo inf on inf.bookingid=bk.pk_seq
									left join Esche sch on sch.pk_seq=bk.scheduleId
									where  CONVERT(varchar(10),GETDATE(),120) <= s.date and bk.clinicid='{id}' and bk.statusId<>4
											and ('{keyword}'='0' or cast(bk.pk_seq as varchar(20)) +' '+bk.code +''+ isnull(u.name,'') like N'%{keyword}%')
									order by bk.createdAt desc, s.date asc,bk.statusId asc,bk.stt asc
							";
					dt = db.getDataTable(query);
					return new ObjectResponse
					{
						result = 1,
						message = "Success",
						content = dt
					};
				}
			}
			catch(Exception e)
            {
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}
			
		}
		public ObjectResponse accessToPay(String userId,String bookingCode)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic,isnull(c.name,'') name  from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									left join Ecli_info c on c.pk_seq=isnull(a.clinicId,b.clinicid)
									where a.roleID in (3) and a.pk_seq='{userId}' and isnull(a.clinicId,b.clinicid) is not null
								";
					String id = "";
					String name = "";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}
					foreach (DataRow row in dt.Rows)
					{
						id = row["clinic"].ToString();
						name = row["name"].ToString();
					}
					db.BeginTran();
					query = $@"Update Ebooking set statusId=2 where pk_seq='{bookingCode}' ";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo booking",
							content = bookingCode
						};
					}

					query = $@"insert into reciept(bookingid,historyid,revenue,createdAt)
							select pk_seq,null,isnull(price,0),GETDATE() from Ebooking where pk_seq='{bookingCode}'";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo booking",
							content = bookingCode
						};
					}
					db.CommitAndDispose();
					return new ObjectResponse
					{
						result = 1,
						message = $@"Success",
						content = bookingCode
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}

		}
		public ObjectResponse getListBookingAccess(String userId,String macode)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic,isnull(c.name,'') name  from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									left join Ecli_info c on c.pk_seq=isnull(a.clinicId,b.clinicid)
									where a.roleID in (2) and a.pk_seq='{userId}' and isnull(a.clinicId,b.clinicid) is not null
								";
					String id = "";
					String name = "";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}
					foreach (DataRow row in dt.Rows)
					{
						id = row["clinic"].ToString();
						name = row["name"].ToString();
					}


					query = $@"select bk.pk_seq as bookingId,bk.code as bookingCode, bk.stt as bookingStt
									, dt.name as DoctorName,u.name as PatientName,s.date , s.timedt as buoi , inf.bloodgr nhommau
									,inf.health_status,inf.taikham as isTaikham,inf.age, bk.price , u.email as emailPatient
									,sch.pk_seq as scheduleId
									from Ebooking bk
									left join Esche s on bk.scheduleId=s.pk_seq
									left join Edt_info dti on s.doctorid=dti.userid
									left join Euser dt on s.doctorid=dt.pk_seq
									left join Euser u on bk.patientId=u.pk_seq
									left join bkinfo inf on inf.bookingid=bk.pk_seq
									left join Esche sch on sch.pk_seq=bk.scheduleId
									where  CONVERT(varchar(10),GETDATE(),120) = s.date and dt.pk_seq='{userId}' and bk.statusId=2
									and ('{macode}'='0' or bk.code +'' +isnull(u.name,'')+u.email like N'%{macode}%') 

									union all

									select bk.pk_seq as bookingId,bk.code as bookingCode, bk.stt as bookingStt
									, dt.name as DoctorName,u.name as PatientName,s.date , s.timedt as buoi , inf.bloodgr nhommau
									,inf.health_status,inf.taikham as isTaikham,inf.age, bk.price , u.email as emailPatient
									,sch.pk_seq as scheduleId
									from Ebooking bk
									left join Esche s on bk.scheduleId=s.pk_seq
									left join Edt_info dti on s.doctorid=dti.userid
									left join Euser dt on s.doctorid=dt.pk_seq
									left join Euser u on bk.patientId=u.pk_seq
									left join bkinfo inf on inf.bookingid=bk.pk_seq
									left join Esche sch on sch.pk_seq=bk.scheduleId
									where  CONVERT(varchar(10),GETDATE(),120) <> s.date and dt.pk_seq='{userId}'  and bk.statusId=2
											and ('{macode}'='0' or bk.code  +isnull(u.name,'')+u.email like N'%{macode}%')
							";
					dt = db.getDataTable(query);
					return new ObjectResponse
					{
						result = 1,
						message = "Success",
						content = dt
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}

		}
		public ObjectResponse getDetailBookingByDoctor(String bookingId, String userId)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									where a.roleID in (2,3) and a.pk_seq='{userId}'
								";
					String id = "";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}

					query = $@"
						select b.pk_seq as bookingId,b.code as code, b.stt as stt, inf.bloodgr nhommau
									, inf.health_status, inf.patientName as namePatient , inf.taikham as isTaikham, inf.age
									, b.price , u.email as emailPatient, sch.date ngaykham,sch.pk_seq as scheduleId
									,dt.name doctorName,b.patientId
								from  Ebooking b
								left join bkinfo inf on inf.bookingid=b.pk_seq
								left join Esche sch on sch.pk_seq=b.scheduleId
								left join Euser dt on dt.pk_seq=sch.doctorid
								left join Euser u on u.pk_seq=b.patientId 
								where b.pk_seq='{bookingId}'";

					dt = db.getDataTable(query);
					
					dt = db.getDataTable(query);
					return new ObjectResponse
					{
						result = 1,
						message = $@"Success",
						content = dt
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}
		}
		public ObjectResponse accessToPayMedicine(String userId, String historyId)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic,isnull(c.name,'') name  from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									left join Ecli_info c on c.pk_seq=isnull(a.clinicId,b.clinicid)
									where a.roleID in (3) and a.pk_seq='{userId}' and isnull(a.clinicId,b.clinicid) is not null
								";
					String id = "";
					String name = "";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}
					foreach (DataRow row in dt.Rows)
					{
						id = row["clinic"].ToString();
						name = row["name"].ToString();
					}
					db.BeginTran();

					query = $@"insert into reciept(bookingid,historyid,revenue,createdAt)
								select a.bookingid,a.pk_seq,sum(b.price),GETDATE()
								from Ehistory a
								left join Ehistory_medicine b on a.pk_seq=b.historyId
								where  a.pk_seq='{historyId}'
								group by a.bookingid,a.pk_seq";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo booking",
							content = historyId
						};
					}
					db.CommitAndDispose();
					return new ObjectResponse
					{
						result = 1,
						message = $@"Success",
						content = historyId
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}

		}
		public ObjectResponse getListBookingByPatient(String userId, String keyword)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"select bk.pk_seq as bookingId,bk.code as bookingCode, bk.stt as bookingStt
									, dt.name as DoctorName,u.name as PatientName,s.date , s.timedt as buoi , inf.bloodgr nhommau
									,inf.health_status,inf.taikham as isTaikham,inf.age, bk.price , u.email as emailPatient
									,sch.pk_seq as scheduleId,cli.name as clinicName,cli.pk_seq as clinicId
									,bk.statusId 									
									,case when bk.statusId=1 then N'Đã xác nhận' when bk.statusId=2 then N'Đã thanh toán' when bk.statusId =4 then N'Đã khám' else '' end as trangthai
									,bk.createdAt
									from Ebooking bk
									left join Esche s on bk.scheduleId=s.pk_seq
									left join Edt_info dti on s.doctorid=dti.userid
									left join Euser dt on s.doctorid=dt.pk_seq
									left join Euser u on bk.patientId=u.pk_seq
									left join bkinfo inf on inf.bookingid=bk.pk_seq
									left join Esche sch on sch.pk_seq=bk.scheduleId
									left join Ecli_info cli on cli.pk_seq=bk.clinicId
									where  bk.patientId ='{userId}'
											and ('{keyword}'='0' or bk.code +'' + s.date+''+s.timedt+isnull(dt.name,'') +isnull(cli.name,'') like N'%{keyword}%')
									order by bk.pk_seq desc,s.date asc,bk.statusId asc,bk.stt asc
							";
					DataTable dt = db.getDataTable(query);
					return new ObjectResponse
					{
						result = 1,
						message = "Success",
						content = dt
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}

		}
		public ObjectResponse getListHistotyByPatient(String userId, String keyword)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"select hs.pk_seq as historyId, hs.code as historycode, bk.pk_seq as bookingId
											, sch.doctorid, dt.name as DoctorName,u.name as PatientName,s.date , s.timedt as buoi , inf.bloodgr nhommau
																			,inf.health_status,inf.taikham as isTaikham,inf.age, bk.price , u.email as emailPatient
																			,sch.pk_seq as scheduleId,cli.name as clinicName,cli.pk_seq as clinicId
											,hs.diagnostic,hs.re_examinationDate,hs.advice
										from Ehistory hs 
										left join Ebooking bk on hs.bookingid=bk.pk_seq
										left join Esche s on bk.scheduleId=s.pk_seq
										left join Edt_info dti on s.doctorid=dti.userid
										left join Euser dt on s.doctorid=dt.pk_seq
										left join Euser u on bk.patientId=u.pk_seq
										left join bkinfo inf on inf.bookingid=bk.pk_seq
										left join Esche sch on sch.pk_seq=bk.scheduleId
										left join Ecli_info cli on cli.pk_seq=bk.clinicId
									where  bk.patientId ='{userId}'
											and ('{keyword}'='0' or hs.code+'' +bk.code +'' + s.date+''+s.timedt+isnull(dt.name,'') +isnull(cli.name,'') like N'%{keyword}%')
									order by bk.pk_seq desc,s.date asc,bk.statusId asc,bk.stt asc
							";
					DataTable dt = db.getDataTable(query);
					return new ObjectResponse
					{
						result = 1,
						message = "Success",
						content = dt
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = null
				};
			}

		}
	}
}