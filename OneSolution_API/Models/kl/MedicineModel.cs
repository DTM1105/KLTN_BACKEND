using OneSolution_API.Managers;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.kl
{
    public class MedicineModel
    {
        public String id { get; set; }
        public String code { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String unit { get; set; }
        public double amout { get; set; }
		public double price { get; set; }
		public String medicineId { get; set; }
		public ObjectResponse createMedicine(List<MedicineModel> medicines,String userId)
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
					else
					{
						db.BeginTran();
						foreach (DataRow row in dt.Rows)
						{
							id = row["clinic"].ToString();
							name= row["name"].ToString();
						}
						foreach(MedicineModel medicine in medicines)
                        {
							query = $@"select * from Emedicine me where me.code='{medicine.code}' and me.clinicid='{id}'";
							dt = db.getDataTable(query);
							if(dt.Rows.Count > 0)
                            {
								db.RollbackAndDispose();
								return new ObjectResponse
								{
									result = 0,
									message = $@"Err: Mã thuốc {medicine.code} của bệnh viện {name} đã tồn tại",
									content = medicines
								};
							}
							query = $@"INSERT INTO [dbo].[Emedicine]
												   ([clinicid]
												   ,[code]
												   ,[name]
												   ,[statusid]
												   ,[description]
												   ,[unit]
												   ,[amount],[price]
												   ,[createdAt])
									select '{id}',N'{medicine.code}',N'{medicine.name}',1,N'{medicine.description}',N'{medicine.unit}',{medicine.amout},{medicine.price},GETDATE()";
							if (!db.updateQuery(query))
							{
								db.RollbackAndDispose();
								return new ObjectResponse
								{
									result = 0,
									message = $@"Err: Lỗi khi thêm thuốc {medicine.name}",
									content = medicines
								};
							}

						}
						db.CommitAndDispose();
						return new ObjectResponse
						{
							result = 1,
							message = $@"Success",
							content = medicines
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
		public ObjectResponse getListMedicine( String userId,String keyword)
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
					else
					{
						foreach (DataRow row in dt.Rows)
						{
							id = row["clinic"].ToString();
							name = row["name"].ToString();
						}

						query = $@"select a.pk_seq as id , code,a.name, statusid , case when statusid=1 then N'Hoạt động' when statusid=2 then N'Ngưng hoạt động' else '' end as trangthaitext
										, clinicid, b.name as clinicName, a.description, a.unit,a.price
								from  Emedicine a
								left join Ecli_info b on a.clinicid=b.pk_seq where a.clinicid='{id}' and ('{keyword}'='0' or cast(a.pk_seq as varchar(20))+' '+a.code+' '+a.name like N'%{keyword}%')
								";
						dt = db.getDataTable(query);
						db.Dispose();
						return new ObjectResponse
						{
							result = 1,
							message = "Success" ,
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
		public ObjectResponse getListMedicinebyId(String medicineId,String userId)
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
					else
					{
						foreach (DataRow row in dt.Rows)
						{
							id = row["clinic"].ToString();
							name = row["name"].ToString();
						}

						query = $@"select a.pk_seq as id , code,a.name, statusid , case when statusid=1 then N'Hoạt động' when statusid=2 then N'Ngưng hoạt động' else '' end as trangthaitext
										, clinicid, b.name as clinicName, a.description, a.unit,a.amount,a.price
								from  Emedicine a
								left join Ecli_info b on a.clinicid=b.pk_seq where a.clinicid='{id}' and a.pk_seq='{medicineId}'
								";
						dt = db.getDataTable(query);
						db.Dispose();
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
		public ObjectResponse getListMedicinebyCodeName(String code, String userId)
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
					else
					{
						foreach (DataRow row in dt.Rows)
						{
							id = row["clinic"].ToString();
							name = row["name"].ToString();
						}

						query = $@"select top 10 a.pk_seq as id , code,a.name, statusid , case when statusid=1 then N'Hoạt động' when statusid=2 then N'Ngưng hoạt động' else '' end as trangthaitext
										, clinicid, b.name as clinicName, a.description, a.unit,a.amount,a.price
								from  Emedicine a
								left join Ecli_info b on a.clinicid=b.pk_seq where a.clinicid='{id}' and ('{code}' ='' or a.code+a.name like N'%{code}%') and a.statusid =1
								";
						dt = db.getDataTable(query);
						db.Dispose();
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
		public ObjectResponse editMedicine(MedicineModel medicine, String userId)
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
					else
					{
						db.BeginTran();
						foreach (DataRow row in dt.Rows)
						{
							id = row["clinic"].ToString();
							name = row["name"].ToString();
						}
						query = $@"select * from Emedicine me where me.code='{medicine.code}' and me.clinicid='{id}' and me.pk_seq <> '{medicine.medicineId}'";
						dt = db.getDataTable(query);
						if (dt.Rows.Count > 0)
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Mã thuốc {medicine.code} của bệnh viện {name} đã tồn tại",
								content = medicine
							};
						}
							query = $@"UPDATE  [dbo].[Emedicine]
												   SET [code] =N'{medicine.code}'
												   ,[name] =N'{medicine.name}'
												   ,[description]=N'{medicine.description}'
												   ,[unit]= N'{medicine.unit}'
												   ,[amount] ={medicine.amout},[price]={medicine.price}
												   ,[updatedAt]=GETDATE() 
									where pk_seq ='{medicine.medicineId}'";
							if (!db.updateQuery(query))
							{
								db.RollbackAndDispose();
								return new ObjectResponse
								{
									result = 0,
									message = $@"Err: Lỗi khi thêm thuốc {medicine.name}",
									content = medicine
								};
							}

						
						db.CommitAndDispose();
						return new ObjectResponse
						{
							result = 1,
							message = $@"Success",
							content = medicine
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
		public ObjectResponse deleteMedicine(MedicineModel medicine, String userId)
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
					else
					{
						db.BeginTran();
						foreach (DataRow row in dt.Rows)
						{
							id = row["clinic"].ToString();
							name = row["name"].ToString();
						}
						query = $@"update Emedicine  set statusid=2 where pk_seq='{medicine.medicineId}'";
						dt = db.getDataTable(query);
						if (!db.updateQuery(query))
						{
							db.RollbackAndDispose();
							return new ObjectResponse
							{
								result = 0,
								message = $@"Err: Lỗi khi xóa thuốc {medicine.code}!! Vui lòng báo admin để thử lại",
								content = medicine
							};
						}
						db.CommitAndDispose();
						return new ObjectResponse
						{
							result = 1,
							message = $@"Success",
							content = medicine
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
		public ObjectResponse getListMedicineActive(String userId, String keyword)
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
					else
					{
						foreach (DataRow row in dt.Rows)
						{
							id = row["clinic"].ToString();
							name = row["name"].ToString();
						}

						query = $@"select top 7 a.pk_seq as id , code,a.name, statusid , case when statusid=1 then N'Hoạt động' when statusid=2 then N'Ngưng hoạt động' else '' end as trangthaitext
										, clinicid, b.name as clinicName, a.description, a.unit,a.price
								from  Emedicine a
								left join Ecli_info b on a.clinicid=b.pk_seq where a.clinicid='{id}' and ('{keyword}'='0' or cast(a.pk_seq as varchar(20))+' '+a.code+' '+a.name like N'%{keyword}%') and a.statusid=1
								";
						dt = db.getDataTable(query);
						db.Dispose();
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
	}
}