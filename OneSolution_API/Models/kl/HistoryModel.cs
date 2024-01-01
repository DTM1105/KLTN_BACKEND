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
    public class HistoryModel
    {
        
        public String id { get; set; }
        public String bookingId { get; set; }
		public String bookingCode { get; set; }
		public String diagnostic { get; set; }
        public String taikhamDate { get; set; }
		public String code { get; set; }
		public String advice { get; set; }
		public List<His_Medicine> medicines { get; set; }
		public ObjectResponse createHistoryByDoctor(HistoryModel history,String userId)
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
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}
					query = $@"select top 1 a.pk_seq as bookingId,a.code as bookingCode, c.pk_seq as clinicId,ISNULL(c.name,'') as clinicName,patient.pk_seq as patientId, patient.name as patientName, patient.email as patientMail
												, CONVERT(varchar(10),GETDATE(),103) as ngaykham, doctor.name as dtName
												,a.code as maKb
										from Ebooking a 
										inner join Esche b on a.scheduleId=b.pk_seq
										left join Ecli_info c on b.clinicid=c.pk_seq
										left join Euser patient on patient.pk_seq=a.patientId
										left join Euser doctor on doctor.pk_seq=b.doctorid
										where a.statusId=2 and  b.doctorid='{userId}' and (a.pk_seq='{history.bookingId}' or a.code='{history.bookingCode}')
								";
					dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = "Booking này đã được thực hiện:",
							content = null
						};
					}
					userModel patient = new userModel();
					userModel doctor = new userModel();
					ClinicModel clinic = new ClinicModel();
					String makb = "";
					String ngaykham = "";


					foreach (DataRow row in dt.Rows)
					{
						makb = row["maKb"].ToString();
						ngaykham = row["ngaykham"].ToString();
						clinic.id = row["clinicId"].ToString();
						clinic.name = row["clinicName"].ToString();
						patient.id = row["patientId"].ToString();
						patient.name = row["patientName"].ToString();
						patient.email = row["patientMail"].ToString();
						doctor.name = row["dtName"].ToString();
						history.bookingId= row["bookingId"].ToString();
						history.bookingCode = row["bookingCode"].ToString(); 
					}


					db.BeginTran();

					query = $@"insert into Ehistory (code,bookingid,diagnostic,re_examinationDate,statusId,createdAt,advice)
								select top 1 code+FORMAT(GETDATE(), 'mmss') as code,e.pk_seq,N'{history.diagnostic}','{history.taikhamDate}',1,GETDATE(),N'{history.advice}'  from Ebooking e where e.pk_seq='{history.bookingId}'
							";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo bệnh án",
							content = history
						};
					}
					query = "select scope_identity()  as tthdId";
					String historyId = "";
					DataTable rsTthd = db.getDataTable(query);
					foreach (DataRow row in rsTthd.Rows)
					{
						historyId = row["tthdId"].ToString();
						break;
					}
					query = $@"select top 1 code from Ehistory where pk_seq='{historyId}'";
					String code = db.getFirstStringValueSqlCatchException(query);
					
					String account = "";
					MedicineModel mdcine = new MedicineModel();
					String html = "";
					String content = "";
					foreach (His_Medicine md in history.medicines)
                    {
                        if (String.IsNullOrEmpty(md.medicineId)|| md.medicineId.Length <=0)
                        {
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Vui lòng chọn thuốc được show ra trong pop up",
								content = history
							};
						}

						query = $@"select code,name,unit,isnull(amount,0) amount, isnull(price,0) price
									from Emedicine where code = '{md.medicineId}' or pk_seq='{md.medicineId}'";
						dt = db.getDataTable(query);
						if(dt.Rows.Count > 0)
                        {
							account = XuLy.ParseDataRowToJson(dt.Rows[0]);
							mdcine = JObject.Parse(account).ToObject<MedicineModel>();


							query = $@"insert into Ehistory_medicine(historyId,medicineId,amount,price)
									select '{historyId}','{md.medicineId}','{md.amount}','{md.amount * mdcine.price}'
									";
							if (!db.updateQuery(query))
							{
								db.RollbackAndDispose();
								return new ObjectResponse
								{
									result = 0,
									message = $@"Err: Lỗi khi tạo đơn thuốc",
									content = history
								};
							}
							html += @" <tr style=""  border: 1px solid black; border - collapse: collapse; "" >";
							html += @" <td style=""  border: 1px solid black; border - collapse: collapse; "" >"+mdcine.code+"</td>";
							html += @" <td style=""  border: 1px solid black; border - collapse: collapse; "" >" + mdcine.name + "</td>";
							html += @" <td style=""  border: 1px solid black; border - collapse: collapse; "" >" + md.amount + "</td>";
							html += @" <td style=""  border: 1px solid black; border - collapse: collapse; "" >" + mdcine.price + "</td>";
							html += @" <td style=""  border: 1px solid black; border - collapse: collapse; "" >" + md.amount * mdcine.price + "</td>";
							html += @" </tr> ";
                        }
                        else
                        {
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Thuốc không có trong hệ thống, vui lòng chọn trong popup",
								content = history
							};
						}
					}
					if(html.Length > 0)
                    {
						content = $@"<table style="" witdh: 50%; border: 1px solid black; border - collapse: collapse; ""  >
										<tr style = ""  border: 1px solid black; border-collapse: collapse;"" >
											<th style = ""  border: 1px solid black; border-collapse: collapse;"" > Mã thuốc </ th >
											<th style = ""  border: 1px solid black; border-collapse: collapse;"" > Tên thuốc </ th >
											<th style = ""  border: 1px solid black; border-collapse: collapse;"" > Số lượng </ th >
											<th style = ""  border: 1px solid black; border-collapse: collapse;"" > Đơn giá </ th >
											<th style = ""  border: 1px solid black; border-collapse: collapse;"" > Thành tiền </ th >
										</tr > {html}  </table>"
										
										;
                    }
					query = $@"update Ebooking set statusId=4 where pk_seq='{history.bookingId}'";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Err: Lỗi khi tạo đơn thuốc",
							content = history
						};
					}
					MailModel.sendMailBenhAn(patient, history, doctor, clinic,code, ngaykham,content);
					history.code = code;
					history.id = historyId;
					db.CommitAndDispose();
					return new ObjectResponse
					{
						result = 1,
						message = $@"Success",
						content = history
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
		public ObjectResponse getDetailHistoryById(String historyId, String userId)
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
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}

					query = $@"
						select h.code as mabenhan,diagnostic,isnull(h.re_examinationDate,'') as re_examinationDate,isnull(inf.patientName,'') patientName,age,isnull(h.advice,'') advice 
						,isnull(cli.name,'') as clinicName,dt.name as doctorName,b.patientId
						from Ehistory h
						left join Ebooking b on h.bookingid=b.pk_seq
						left join bkinfo inf on inf.bookingid=b.pk_seq
						left join Esche sch on sch.pk_seq=b.scheduleId
						left join Euser dt on dt.pk_seq=sch.doctorid
						left join Ecli_info cli on cli.pk_seq=b.clinicId 
						where h.pk_seq='{historyId}' and sch.doctorId='{userId}'";
					dt = db.getDataTable(query);
					String historyCode = "";
					String diagnostic = "";
					String re_examinationDate = "";
					String patientName = "";
					String age = "";
					String advice = "";
					String clinicName = "";
					String doctorName = "";
					String patientId = "";
					foreach (DataRow row in dt.Rows)
                    {
						historyCode = row["mabenhan"].ToString();
						diagnostic = row["diagnostic"].ToString();
						re_examinationDate = row["re_examinationDate"].ToString();
						patientName = row["patientName"].ToString();
						age = row["age"].ToString();
						advice= row["advice"].ToString();
						clinicName = row["clinicName"].ToString();
						doctorName = row["doctorName"].ToString();
						patientId = row["patientId"].ToString();
					}

					query = $@"select m.code mathuoc,m.name,m.description, m.amount solieungay, m.unit,isnull(m.price,0) as dongia
						,isnull(m.price,0)* hm.amount as renevue,hm.amount as soluong
						from Ehistory h
						left join Ehistory_medicine hm on h.pk_seq=hm.historyId
						left join Emedicine m on m.pk_seq=hm.medicineId
					
						where h.pk_seq='{historyId}'";
					dt = db.getDataTable(query);
					return new ObjectResponse
					{
						result = 1,
						message = $@"Success",
						content = new
                        {
							historyCode=historyCode,
							diagnostic= diagnostic,
							re_examinationDate= re_examinationDate,
							patientName= patientName,
							age= age,
							medicines=dt,
							advice=advice,
							clinicName=clinicName,
							doctorName=doctorName,
							patientId= patientId
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
		public ObjectResponse getListHistoryByDoctor( String userId,String keyword)
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
							message = "Bạn không có phân quyền để thực hiện nghiệp vụ này:",
							content = null
						};
					}

					query = $@"
						select h.pk_seq as historyId,h.code as mabenhan,diagnostic,h.re_examinationDate,isnull(inf.patientName,'') patientName 
							, h.statusId , case when (select count(1) from reciept rc where rc.historyid=h.pk_seq) >0 then N'Đã khám, Đã mua thuốc' else N'Đã khám' end as status
						from Ehistory h
						left join Ebooking b on h.bookingid=b.pk_seq
						left join Esche sch on sch.pk_seq=b.scheduleId
						left join bkinfo inf on inf.bookingid=b.pk_seq
						where  b.statusId=4 and sch.doctorId='{userId}' and('{keyword}'='0' or  cast(b.patientId as varchar(20))+'-'+isnull(h.code,'')+'-'+isnull(b.code,'')+'-'+isnull(inf.patientName,'') like N'%{keyword}%')";
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
		public ObjectResponse getListHistoryByAdminClinic(String userId,String keyword)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									where a.roleID in (3) and a.pk_seq='{userId}'
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
						select h.pk_seq as historyId,h.code as historycode,dt.name as doctorName,diagnostic,h.re_examinationDate,isnull(inf.patientName,'') patientName 
							, isnull(h.statusId,0)  as historystatusId
							, case when (select count(1) from reciept rc where rc.historyid=h.pk_seq) >0 then N'Đã khám, Đã mua thuốc' else N'Đã khám' end as status
							, case when (select count(1) from reciept rc where rc.historyid=h.pk_seq) >0 then 1 else 0 end as thanhtoanthuoc
							from Ehistory h
							left join Ebooking b on h.bookingid=b.pk_seq
							left join bkinfo inf on inf.bookingid=b.pk_seq
							left join Esche sch on sch.pk_seq=b.scheduleId
							left join Euser dt on dt.pk_seq=sch.doctorid
							where  b.statusId=4 and exists (select 1 from Euser patient where sch.clinicid=patient.clinicId and patient.pk_seq='{userId}')
									and('{keyword}'='0' or  cast(b.patientId as varchar(20))+'-'+ isnull(h.code,'')+'-'+isnull(b.code,'')+'-'+isnull(inf.patientName,'')+'-'+isnull(dt.name,'') like N'%{keyword}%')";
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
		public ObjectResponse getDetailHistoryByAdminClinic(String historyId, String userId)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select isnull(a.clinicId,b.clinicid) clinic from Euser a
									left join Edt_info b on a.pk_seq=b.userid
									where a.roleID in (3) and a.pk_seq='{userId}'
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
						select h.code as mabenhan,diagnostic,dt.name as doctorName,isnull(h.re_examinationDate,'') as re_examinationDate,isnull(inf.patientName,'') patientName,age, isnull(h.advice,'') advice
								,isnull(cli.name,'') as clinicName 
							, case when (select count(1) from reciept rc where rc.historyid=h.pk_seq) >0 then N'Đã khám, Đã mua thuốc' else N'Đã khám' end as status
							, case when (select count(1) from reciept rc where rc.historyid=h.pk_seq) >0 then 1 else 0 end as thanhtoanthuoc
								from Ehistory h
								left join Ebooking b on h.bookingid=b.pk_seq
								left join bkinfo inf on inf.bookingid=b.pk_seq
								left join Esche sch on sch.pk_seq=b.scheduleId
								left join Euser dt on dt.pk_seq=sch.doctorid
								left join Ecli_info cli on cli.pk_seq=b.clinicId 
								where  h.pk_seq='{historyId}' and exists (select 1 from Euser patient where sch.clinicid=patient.clinicId and patient.pk_seq='{userId}')";

					dt = db.getDataTable(query);
					String historyCode = "";
					String diagnostic = "";
					String re_examinationDate = "";
					String patientName = "";
					String age = "";
					String doctorName = "";
					String advice = "";
					String clinicName = "";
					String thanhtoanthuoc = "";
					foreach (DataRow row in dt.Rows)
					{
						historyCode = row["mabenhan"].ToString();
						diagnostic = row["diagnostic"].ToString();
						re_examinationDate = row["re_examinationDate"].ToString();
						patientName = row["patientName"].ToString();
						age = row["age"].ToString();
						doctorName = row["doctorName"].ToString();
						advice = row["advice"].ToString();
						clinicName = row["clinicName"].ToString();
						thanhtoanthuoc= row["thanhtoanthuoc"].ToString();
					}

					query = $@"select m.code mathuoc,m.name,m.description, m.amount solieungay, m.unit,isnull(m.price,0) as dongia
						,isnull(m.price,0)* hm.amount as renevue  ,hm.amount as soluong
						from Ehistory h
						left join Ehistory_medicine hm on h.pk_seq=hm.historyId
						left join Emedicine m on m.pk_seq=hm.medicineId
						where h.pk_seq='{historyId}'";
					dt = db.getDataTable(query);
					return new ObjectResponse
					{
						result = 1,
						message = $@"Success",
						content = new
						{
							historyCode = historyCode,
							diagnostic = diagnostic,
							re_examinationDate = re_examinationDate,
							patientName = patientName,
							age = age,
							doctorName= doctorName,
							medicines = dt,
							advice = advice,
							clinicName= clinicName,
							thanhtoanthuoc= thanhtoanthuoc
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

	}
}