using Newtonsoft.Json.Linq;
using OneSolution_API.Managers;
using OneSolution_API.Models.Login;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static OneSolution_API.Test.DoctorController;

namespace OneSolution_API.Models.kl
{
    public class DoctorModel
    {
        public String doctorId { get; set; }
        public String short_description { get; set; }
        public String short_description_html { get; set; }
        public String description { get; set; }
        public String description_html { get; set; }
        public String clinicid { get; set; }
        public String hocviid { get; set; }
		public List<String> specialtyId { get; set; }
		public String image { get; set; }
		public String name { get; set; }
		public String phone { get; set; }
		public String ngaysinh { get; set; }
		public Double price { get; set; }
		public ObjectResponse createDoctor(DoctorModel doctor)
		{
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();
					String query = $@" select clinicId from Euser where pk_seq = '{doctor.doctorId}' and roleId=2";
					if (Double.IsNaN(doctor.price)) {
						doctor.price = 0;
					}

					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"DoctorId này không có trong hệ thống",
							content = doctor
						};
					}
					Double clinic = db.getFirstDoubleValueSqlCatchException(query);
					query = $@" delete Edt_info where userid='{doctor.doctorId}'";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi thêm thông tin của bác sĩ  {doctor.doctorId}",
							content = doctor
						};
					}
					query = $@" delete Edoctor_spe where doctorid='{doctor.doctorId}'";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1.1: Lỗi khi thêm thông tin của bác sĩ  {doctor.doctorId}",
							content = doctor
						};
					}

					query = $@" insert into Edt_info(userid,	short_description,	short_description_html,	description	,description_html,	hocviid,price,clinicId)
                                select N'{doctor.doctorId}',N'{doctor.short_description}',N'{doctor.short_description_html}',N'{doctor.description}',N'{doctor.description}',1,{doctor.price},{clinic}
                            ";

					

					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L2: Lỗi khi tạo bác sĩ {doctor.doctorId}",
							content = doctor
						};
					}

					query = $@" update Euser set name=N'{doctor.name}',phonenumber='{doctor.phone}',avtimage='{doctor.image}' where pk_seq='{doctor.doctorId}'
                            ";



					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L3: Lỗi khi tạo bác sĩ {doctor.doctorId}",
							content = doctor
						};
					}

					foreach (String x in doctor.specialtyId)
                    {
						if(String.IsNullOrEmpty(x) || x.Length == 0)
                        {
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Vui lòng chọn chuyên khoa",
								content = doctor
							};
						}
						query = $@" insert into Edoctor_spe(doctorid,	speid)
														select '{doctor.doctorId}','{x}'
									";
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"L4: Lỗi khi tạo bác sĩ {doctor.doctorId}",
								content = doctor
							};
						}
					}


					query = $@"select top 1 a.pk_seq as Id,a.name as doctorname, b.description,b.description_html,b.short_description,b.short_description_html
										,b.clinicid,c.name as clinicname,hv.ten as hocvi, b.hocviid
								from Euser a
								inner join Edt_info b on a.pk_seq=b.userid
								left join Ecli_info c on b.clinicid=c.pk_seq
								left join Ehv hv on hv.id=b.hocviid
								where a.pk_seq ='{doctor.doctorId}'
								";
					DataTable dt = db.getDataTable(query);

					query = $@"select b.pk_seq as speId, b.name as speName
								from Edoctor_spe a 
								inner join Espe_info b on a.speid=b.pk_seq
								where a.doctorid ='{doctor.doctorId}'
								";
					DataTable dtSPe = db.getDataTable(query);
					db.CommitAndDispose();
					foreach (DataRow row in dt.Rows)
                    {
						return new ObjectResponse
						{
							result = 1,
							message = "Success",
						};
					}
					return new ObjectResponse
					{
						result = 1,
						message = "Không có dữ liệu",
						content = null
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = doctor
				};
			}
		}
		public ObjectResponse getListDoctor()
        {
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();
					
					String query = $@"select a.pk_seq as Id,a.name as doctorname, b.description,b.description_html,b.short_description,b.short_description_html
										,b.clinicid,c.name as clinicname,hv.ten as hocvi, b.hocviid,a.avtimage
										, (select count(1) from reciept r inner join Ebooking b on r.bookingid=b.pk_seq inner join Esche s on s.pk_seq=b.scheduleId where s.doctorId=a.pk_seq) as cnt
										,b.price,cli.address
								from Euser a
								left join Edt_info b on a.pk_seq=b.userid
								left join Ecli_info c on b.clinicid=c.pk_seq
								left join Ehv hv on hv.id=b.hocviid
								left join Ecli_info cli on cli.pk_seq=a.clinicId
								where a.roleId=2 order by cnt desc
								";
					DataTable dt = db.getDataTable(query);
					db.CommitAndDispose();
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
		public ObjectResponse getDoctorById(String doctorid)
		{
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();

					String query = $@"select a.pk_seq as Id,a.name as doctorname, b.description,b.description_html,b.short_description,b.short_description_html
										,b.clinicid,c.name as clinicname,hv.ten as hocvi, b.hocviid,isnull(a.avtimage,'') avtimage,a.email,isnull(a.phonenumber,'') phonenumber,isnull(a.ngaysinh,'') ngaysinh,isnull(b.price,0) price
										,c.address 
								from Euser a
								left join Edt_info b on a.pk_seq=b.userid
								left join Ecli_info c on b.clinicid=c.pk_seq
								left join Ehv hv on hv.id=b.hocviid
								where a.roleId=2 and a.pk_seq='{doctorid}'
								";
					DataTable dt = db.getDataTable(query);
					query = $@"select b.pk_seq as speId, b.name as speName
								from Edoctor_spe a 
								inner join Espe_info b on a.speid=b.pk_seq
								where a.doctorid ='{doctorid}'
								";
					DataTable dtSPe = db.getDataTable(query);


					db.CommitAndDispose();
					foreach (DataRow row in dt.Rows)
					{
						return new ObjectResponse
						{
							result = 1,
							message = "Success",
							content = new
							{
								Id = int.Parse(row["Id"].ToString()),
								doctorname = row["doctorname"].ToString(),
								description = row["description"].ToString(),
								description_html = row["description_html"].ToString(),
								short_description = row["short_description"].ToString(),
								short_description_html = row["short_description_html"].ToString(),
								clinicid = int.Parse(row["clinicid"].ToString()),
								clinicname = row["clinicname"].ToString(),
								hocvi = row["hocvi"].ToString(),
								hocviid = int.Parse(row["hocviid"].ToString()),
								specialtyId = dtSPe,
								image = row["avtimage"].ToString(),
								email= row["email"].ToString(),
								phone= row["phonenumber"].ToString(),
								ngaysinh= row["ngaysinh"].ToString(),
								price=Double.Parse(row["price"].ToString()),
								address = row["address"].ToString()
							}
						};
					}
					return new ObjectResponse
					{
						result = 0,
						message = "Không có dataa:" ,
						content = null
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
		public ObjectResponse createSchedule(ScheduleModel sche)
		{
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();
					String query = $@" select d.clinicid from Euser e
										left join Edt_info d on e.pk_seq=d.userid
										where e.pk_seq = '{sche.doctorId}' and e.roleId=2";


					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"DoctorId này không có trong hệ thống",
							content = sche
						};
					}
					String account = XuLy.ParseDataRowToJson(dtc.Rows[0]);
					ScheduleModel guest = JObject.Parse(account).ToObject<ScheduleModel>();
					String timeList = "10,";
					if(sche.time.Count > 0)
                    {
						foreach (String time in sche.time)
						{
							String timedt = "";
							timeList += time + ",";
							if (time.Equals("0"))
							{
								timedt = "Sáng";
							}
							else
							{
								timedt = "Chiều";
							}
							query = $@" select * from Esche where doctorid='{sche.doctorId}' and date='{sche.date}' and timeid='{time}'";
							dtc = db.getDataTable(query);
							if (dtc.Rows.Count > 0)
							{
								continue;
							}
							query = $@" insert into Esche(clinicid, doctorid, date, timeid,timedt,createdAt)
									select '{guest.clinicId}','{sche.doctorId}','{sche.date}','{time}',N'{timedt}',GETDATE()";
							if (!db.updateQuery(query))
							{
								db.RollbackAndDispose();
								return new ObjectResponse
								{
									result = 0,
									message = $@"L2: Lỗi khi tạo bác sĩ {sche.doctorId}",
									content = sche
								};
							}
						}
                    }
					query = $@"select * from Esche e where e.doctorid='{sche.doctorId}' and e.date='{sche.date}' and e.timeid not in ({timeList.Substring(0, timeList.Length - 1)}) 
								and exists (select 1 from Ebooking bk where bk.scheduleId=e.pk_seq)";
					dtc = db.getDataTable(query);
					if(dtc.Rows.Count > 0)
                    {
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L2: Lịch hẹn bạn xóa đã có người đặt vui lòng thử lại",
							content = sche
						};
					}
					query = $@" delete Esche where doctorid='{sche.doctorId}' and date='{sche.date}' and timeid not in ({timeList.Substring(0, timeList.Length - 1)}) ";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1.1: Lỗi khi tạo thông tin lịch hẹn của bác sĩ  {sche.doctorId}",
							content = sche
						};
					}

					
					db.CommitAndDispose();
					return new ObjectResponse
					{
						result = 1,
						message = "Success",
						content = sche
					};
				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = sche
				};
			}
		}
		public ObjectResponse getListDoctorbyToken(String userId)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									where a.roleID in (2,3) and a.pk_seq='{userId}'

									union all

									select 1 as clinic from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									where a.roleID in (0) and a.pk_seq='{userId}'
								";
					String id = "";
					DataTable dt = db.getDataTable(query);
                    if (dt.Rows.Count <= 0)
                    {
						return new ObjectResponse
						{
							result = 0,
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:" ,
							content = null
						};
                    }
                    else
                    {
						foreach(DataRow row in dt.Rows)
                        {
							id = row["clinic"].ToString();
                        }
                        if (id.Equals("0"))
                        {
							query = $@"select a.pk_seq as Id,a.name as doctorname, b.description,b.description_html,b.short_description,b.short_description_html
										,isnull(a.clinicid,b.clinicid) clinicid,c.name as clinicname,hv.ten as hocvi, b.hocviid,a.avtimage,a.ngaysinh,a.email,a.phonenumber
										,isnull(b.price,0) price
								from Euser a
								left join Edt_info b on a.pk_seq=b.userid
								left join Ecli_info c on b.clinicid=c.pk_seq
								left join Ehv hv on hv.id=b.hocviid
								where a.roleId=2
								";
							dt = db.getDataTable(query);
							return new ObjectResponse
							{
								result = 1,
								message = "Success",
								content = dt
							};
						}
                        else if(!id.Equals(""))
                        {
							query = $@"select a.pk_seq as Id,a.name as doctorname, b.description,b.description_html,b.short_description,b.short_description_html
										,isnull(a.clinicid,b.clinicid) clinicid,c.name as clinicname,hv.ten as hocvi, b.hocviid,a.avtimage,a.ngaysinh,a.email,a.phonenumber
										,isnull(b.price,0) price
								from Euser a
								left join Edt_info b on a.pk_seq=b.userid
								left join Ecli_info c on b.clinicid=c.pk_seq
								left join Ehv hv on hv.id=b.hocviid
								where a.roleId=2 and a.clinicid ='{id}'
								";
							dt = db.getDataTable(query);
							return new ObjectResponse
							{
								result = 1,
								message = "Success",
								content = dt
							};
						}
						dt = db.getDataTable(query);
						return new ObjectResponse
						{
							result = 1,
							message = "Success",
							content = new List<String>()
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
		public ObjectResponse getListScheduleByDoctorAndDate(String doctorId,String date)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@" select pk_seq as id, clinicid,doctorid,date,timeid,timedt from Esche where doctorid='{doctorId}' and date='{date}'
								";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Success",
							content = new List<String>()
						};
					}
					else
					{
						return new ObjectResponse
						{
							result = 1,
							message = "Success",
							content = dt
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
		public ObjectResponse getListSchedulebyDates(String doctorId, String fromdate,String endDate)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@" select pk_seq as id, clinicid,doctorid,date,timeid,timedt from Esche where doctorid='{doctorId}' and date>='{fromdate}' and date <='{endDate}'
								";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Success",
							content = new List<String>()
						};
					}
					else
					{
						return new ObjectResponse
						{
							result = 1,
							message = "Success",
							content = dt
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
		public ObjectResponse getListDoctorBySpecialty(String specialtyId)
		{
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();

					String query = $@"select a.pk_seq as Id,a.name as doctorname, b.description,b.description_html,b.short_description,b.short_description_html
										,b.clinicid,c.name as clinicname,hv.ten as hocvi, b.hocviid,a.avtimage,b.price,cli.address
								from Euser a
								left join Edt_info b on a.pk_seq=b.userid
								left join Ecli_info c on b.clinicid=c.pk_seq
								left join Ehv hv on hv.id=b.hocviid
								left join Ecli_info cli on cli.pk_seq=a.clinicId
								where a.roleId=2 and exists (select 1 from Edoctor_spe spe where spe.doctorId = a.pk_seq and spe.speid='{specialtyId}')  
								";
					DataTable dt = db.getDataTable(query);
					db.CommitAndDispose();
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
		public ObjectResponse getListDoctorByClinic(String clinicId)
		{
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();

					String query = $@"select a.pk_seq as Id,a.name as doctorname, b.description,b.description_html,b.short_description,b.short_description_html
										,a.clinicId,c.name as clinicname,hv.ten as hocvi, b.hocviid,a.avtimage,b.price,cli.address
								from Euser a
								left join Edt_info b on a.pk_seq=b.userid
								left join Ecli_info c on b.clinicid=c.pk_seq
								left join Ehv hv on hv.id=b.hocviid
								left join Ecli_info cli on cli.pk_seq=a.clinicId
								where a.roleId=2 and a.clinicId='{clinicId}'
								";
					DataTable dt = db.getDataTable(query);
					db.CommitAndDispose();
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
		public ObjectResponse getDoanhThu(String userId,String doctorId, String fromDate, String toDate)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"select sum(isnull(a.revenue,0)) revenue from reciept a
										inner join Ebooking b on a.bookingid=b.pk_seq
										left join Esche c on b.scheduleId=c.pk_seq
										where isnull(a.historyid,0)=0 and c.doctorId='{userId}' and c.date >='{fromDate}' and c.date <='{toDate}'
								";
					Double doanhthulichhen = db.getFirstDoubleValueSqlCatchException(query);

					query = $@"select sum(isnull(a.revenue,0)) revenue from reciept a
										inner join Ebooking b on a.bookingid=b.pk_seq
										left join Esche c on b.scheduleId=c.pk_seq
										where isnull(a.historyid,0) > 0 and c.doctorId='{userId}' and c.date >='{fromDate}' and c.date <='{toDate}'
								";
					 Double doanhthuthuoc = db.getFirstDoubleValueSqlCatchException(query);

					query = $@"select sum(isnull(a.revenue,0)) revenue from reciept a
										inner join Ebooking b on a.bookingid=b.pk_seq
										left join Esche c on b.scheduleId=c.pk_seq
										where c.doctorId='{userId}' and c.date >='{fromDate}' and c.date <='{toDate}'
								";
					Double tongdoanhthu = db.getFirstDoubleValueSqlCatchException(query);

					return new ObjectResponse
					{
						result = 1,
						message = "Success",
						content = new
                        {
							doanhthulichhen= doanhthulichhen,
							doanhthuthuoc= doanhthuthuoc,
							tongdoanhthu= tongdoanhthu
						}
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
		public ObjectResponse getBookingInfo(String userId, String doctorId, String fromDate, String toDate)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"select count(1) from Ebooking b
										left join Esche c on b.scheduleId=c.pk_seq
										where b.statusId = 2  and c.doctorId='{userId}' and c.date >='{fromDate}' and c.date <='{toDate}'
								";
					Double lichhendathanhtoan = db.getFirstDoubleValueSqlCatchException(query);

					query = $@"select count(1) from Ebooking b
										left join Esche c on b.scheduleId=c.pk_seq
										where b.statusId = 4  and c.doctorId='{userId}' and c.date >='{fromDate}' and c.date <='{toDate}'
								";
					Double lichhendakham = db.getFirstDoubleValueSqlCatchException(query);

					query = $@"select count(1) from Ebooking b
										left join Esche c on b.scheduleId=c.pk_seq
										where b.statusId = 1  and  c.doctorId='{userId}' and c.date >='{fromDate}' and c.date <='{toDate}'
								";
					Double lichhendaxacnhan = db.getFirstDoubleValueSqlCatchException(query);

					query = $@"select count(1) from Ebooking b
										left join Esche c on b.scheduleId=c.pk_seq
										where  c.doctorId='{userId}' and c.date >='{fromDate}' and c.date <='{toDate}'
								";
					Double tonglichhen = db.getFirstDoubleValueSqlCatchException(query);

					return new ObjectResponse
					{
						result = 1,
						message = "Success",
						content = new
						{
							lichhendathanhtoan = lichhendathanhtoan,
							lichhendakham = lichhendakham,
							lichhendaxacnhan = lichhendaxacnhan,
							tonglichhen= tonglichhen
						}
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
		public ObjectResponse getDataLichHen(String userId, String doctorId, String fromDate, String toDate)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"	
									
								WITH DateRangeCTE AS (
									SELECT CAST('{fromDate}' AS DATE) AS DateValue
									UNION ALL
									SELECT DATEADD(DAY, 1, DateValue)
									FROM DateRangeCTE
									WHERE DateValue < '{toDate}'
								)
								SELECT DateValue into #a
								FROM DateRangeCTE;

								select convert(varchar(10),a.DateValue,120) as date,sum(a.solichhen) solichhen 
								from(select a.DateValue,sum(case when isnull(c.pk_seq,0)=0 then 0 else 1 end ) as solichhen from #a a
								left join Esche b on a.DateValue =b.date
								left join Ebooking c on b.pk_seq = c.scheduleId
								where  b.doctorId='{userId}' 
								group by a.DateValue

								union all

								select a.DateValue,0 solichhen from #a a
								) a group by a.DateValue
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
		public ObjectResponse getDataLoaiThuoc(String userId, String doctorId, String fromDate, String toDate)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"	
									select top 10 c.name,sum(a.amount) soluongbanra from Ehistory_medicine a
											inner join Ehistory b on a.historyId=b.pk_seq
											left join Emedicine c on a.medicineId=c.pk_seq
											left join Ebooking bk on bk.pk_seq=b.bookingid
											left join Esche sch on sch.pk_seq=bk.scheduleId
										where sch.doctorid ='{userId}' and sch.date >='{fromDate}' and sch.date <='{toDate}'
												and exists (select 1 from reciept rs where rs.historyId=b.pk_seq)
										group by c.name order by soluongbanra desc

									
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
		public ObjectResponse getScheduleToChange(String userId, String doctorId)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"	select a.pk_seq as scheduleId,a.date,a.timeid,a.timedt
											from Esche a
											where convert(varchar(19),DATEADD(HOUR, 1, GETDATE()),126) <= case when a.timeid=0 THEN CONVERT(VARCHAR(10), a.date, 126) + 'T07:00:00' else CONVERT(VARCHAR(10), a.date, 126) + 'T13:00:00' end 
													and a.date <= convert(varchar(10),DATEADD(DAY, 5, GETDATE()),120) 
													and a.doctorId='{doctorId}'
											order by a.date,a.timeid

									
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
		public ObjectResponse getDoctorFreeTime(String userId,String scheduleId)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"	select a.pk_seq as doctorId,a.name as doctorName 
											from Euser a
											where a.roleID=2 and a.trangthai=1 and not exists (select 1 from Esche e where e.date in (select date from Esche where pk_seq={scheduleId}) 
																										and timeid =(select timeid from Esche where pk_seq={scheduleId}) 
																										and e.doctorid=a.pk_seq
																			) and a.clinicId in (select x.clinicid from Esche x where pk_seq='{scheduleId}')

									
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
		public ObjectResponse createChangeScheduleByDoctor(changeSchedule changeSchedule)
		{
			try
			{
				using (var db = new clsDB())
				{
					string query = $@"select a.pk_seq as scheduleId,a.date,a.timeid,a.timedt
											from Esche a
											where convert(varchar(19),DATEADD(HOUR, 1, GETDATE()),126) <= case when a.timeid=0 THEN CONVERT(VARCHAR(10), a.date, 126) + 'T07:00:00' else CONVERT(VARCHAR(10), a.date, 126) + 'T13:00:00' end 
													and a.date <= convert(varchar(10),DATEADD(DAY, 5, GETDATE()),120) 
													and a.doctorId='{changeSchedule.fromDoctorId}' and a.pk_seq='{changeSchedule.scheduleId}'";
					
					DataTable dt = db.getDataTable(query);
					if(dt.Rows.Count <= 0)
                    {
						return new ObjectResponse
						{
							result = 0,
							message = "Lịch hẹn này đã quá giờ đăng kí, vui lòng chọn lịch hẹn khác",
							content = dt
						};
					}
					query = $@"	select a.pk_seq as doctorId,a.name as doctorName 
											from Euser a
											where a.roleID=2 and a.trangthai=1 and not exists (select 1 from Esche e where e.date in (select date from Esche where pk_seq={changeSchedule.scheduleId}) 
																										and timeid =(select timeid from Esche where pk_seq={changeSchedule.scheduleId}) 
																										and e.doctorid=a.pk_seq
																			) and a.clinicId in (select x.clinicid from Esche x where pk_seq='{changeSchedule.scheduleId}') and a.pk_seq = '{changeSchedule.toDoctorId}'

									
								";
					dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Bác sĩ này đã có lịch hẹn rồi vui lòng chọn lịch khác",
							content = dt
						};
					}
					db.BeginTran();
					query = $@" insert into Echangesche(scheduleid,clinicid,fromdoctorid,todoctorid,reason,createdAt,statusid)
										select pk_seq,clinicid,'{changeSchedule.fromDoctorId}','{changeSchedule.toDoctorId}',N'{changeSchedule.reason}',GETDATE(),0 from Esche where pk_seq ='{changeSchedule.scheduleId}'
									";
                    if (!db.updateQuery(query))
                    {
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = "lỗi",
							content = query
						};
					}
					db.CommitAndDispose();
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
		public ObjectResponse approcheChangeSchedule(String userId, String changeId)
		{
			try
			{
				using (var db = new clsDB())
				{
					string query = $@"select * from Echangesche where pk_seq='{changeId}'";
					DataTable dt = db.getDataTable(query);
					if(dt.Rows.Count <= 0)
                    {
						return new ObjectResponse
						{
							result = 0,
							message = "Không có dữ liệu kiểm tra lại ",
							content = dt
						};
					}
					String account = XuLy.ParseDataRowToJson(dt.Rows[0]);
					changeSchedule guest = JObject.Parse(account).ToObject<changeSchedule>();

					query = $@"select a.pk_seq as scheduleId,a.date,a.timeid,a.timedt
											from Esche a
											where convert(varchar(19),DATEADD(HOUR, 1, GETDATE()),126) <= case when a.timeid=0 THEN CONVERT(VARCHAR(10), a.date, 126) + 'T07:00:00' else CONVERT(VARCHAR(10), a.date, 126) + 'T13:00:00' end 
													and a.date <= convert(varchar(10),DATEADD(DAY, 5, GETDATE()),120) 
													and a.doctorId='{guest.fromDoctorId}' and a.pk_seq='{guest.scheduleId}'";

					dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Lịch hẹn này đã quá giờ đăng kí, vui lòng chọn lịch hẹn khác",
							content = dt
						};
					}

					query = $@"	select a.pk_seq as doctorId,a.name as doctorName 
											from Euser a
											where a.roleID=2 and a.trangthai=1 and not exists (select 1 from Esche e where e.date in (select date from Esche where pk_seq={guest.scheduleId}) 
																										and timeid =(select timeid from Esche where pk_seq={guest.scheduleId}) 
																										and e.doctorid=a.pk_seq
																			) and a.clinicId in (select x.clinicid from Esche x where pk_seq='{guest.scheduleId}') and a.pk_seq = '{guest.toDoctorId}'

									
								";
					dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Bác sĩ này đã có lịch hẹn rồi vui lòng chọn lịch khác",
							content = dt
						};
					}
					query = $@" select bk.code as bookingcode,cli.name as clinicName, s.date,s.timedt
										,u.email as mailPatient, isnull(bki.patientName,u.name) as patientName
										,isnull(dt.name,'') dtName, tdt.name todoctorName,isnull(change.reason,'') reason
										,s.pk_seq as scheduleId
								from Ebooking bk
								left join bkinfo bki on bki.bookingid=bk.pk_seq
								left join Esche s on bk.scheduleId=s.pk_seq
								left join Euser u on u.pk_seq=bk.patientId
								left join Echangesche change on s.pk_seq=change.scheduleid
								left join Euser dt on change.fromdoctorid=dt.pk_seq
								left join Euser tdt on tdt.pk_seq=change.todoctorid
								left join Ecli_info cli on cli.pk_seq=change.clinicid
								where change.pk_seq='{changeId}'";
					dt = db.getDataTable(query);
					String todoctorName = "";
					foreach (DataRow row in dt.Rows)
                    {
						MailModel.sendMailChangeSchedule(row);
						todoctorName = row["todoctorName"].ToString();
					}
					db.BeginTran();
					query = $@" update Echangesche set statusid =1 ,adminapproche ='{userId}', adminapprocheTime=GETDATE() where pk_seq= '{changeId}'";
                    if (!db.updateQuery(query))
                    {
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = "Lỗi trong quá trình đổi trạng thái",
							content = dt
						};
					}
					query = $@" update Esche set doctorid='{guest.toDoctorId}' where pk_seq='{guest.scheduleId}'";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = "Lỗi trong quá trình đổi lịch hẹn",
							content = dt
						};
					}
					query = $@" update Ebooking set doctorid='{guest.toDoctorId}' where scheduleId='{guest.scheduleId}'";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = "Lỗi trong quá trình đổi lịch hẹn",
							content = dt
						};
					}
					db.CommitAndDispose();
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
		public ObjectResponse getListChangeSchedule(String userId, String keyword)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@"	select a.pk_seq as changeId, a.scheduleid,a.fromdoctorid,a.todoctorid,a.reason,a.statusid,fromdoctor.name as fromdoctorName, todoctor.name as todoctorName
											, case when statusid =0 then N'Chưa duyệt' else N'Đã duyệt' end  as status,c.date,c.timedt
											from Echangesche a
											left join Euser fromdoctor on a.fromdoctorid =fromdoctor.pk_seq
											left join Euser todoctor on a.todoctorid =todoctor.pk_seq
											left join Esche c on c.pk_seq=scheduleid 
											where ('{keyword}'='0' or isnull(fromdoctor.name,'0') + isnull(todoctor.name,'') + cast(a.pk_seq as varchar(20)) like N'%{keyword}%') 
											and	exists (select 1 from Euser u where a.clinicId= u.clinicId and u.pk_seq='{userId}') order by a.pk_seq desc
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