using OneSolution_API.Managers;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.kl
{
    public class ClinicModel
    {
        public String id { get; set; }
        public String maClinic { get; set; }
        public String trangthaiid { get; set; }
        public String name { get; set; }
        public String shortName { get; set; }
        public String address { get; set; }
        public String short_description { get; set; }
        public String short_description_html { get; set; }
        public String description { get; set; }
        public String description_html { get; set; }
        public String hotline { get; set; }
		public String image { get; set; }

        public ObjectResponse createClinic(ClinicModel clinic)
        {
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();
					String query = $@" select * from Ecli_info where ma = '{clinic.maClinic}'";


					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count > 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Mã này đã tồn tại trong hệ thống",
							content = clinic
						};
					}


					query = $@" insert into Ecli_info(ma,	name,	trangthai	,shortName	,address,short_description,	short_description_html	,description,	description_html	,hotline,image	,createdAt)
                                select N'{clinic.maClinic}',N'{clinic.name}',1,N'{clinic.shortName}',N'{clinic.address}',N'{clinic.short_description}',N'{clinic.short_description_html}',N'{clinic.description}',N'{clinic.description_html}','{clinic.hotline}','{clinic.image}',GETDATE()
                            ";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi tạo phòng khám {clinic.name}",
							content = clinic
						};
					}

					query = "select scope_identity()  as id";
					String id = db.getFirstStringValueSqlCatchException(query);
					if (String.IsNullOrEmpty(id))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L2: Lỗi khi tạo phòng khám {clinic.name}",
							content = clinic
						};
					}
					

					query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, N'Hoạt động' trangthai	,shortName	,address,short_description,	short_description_html	,description,	description_html	,hotline,image	 from Ecli_info where pk_seq={id}";
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
					content = clinic
				};
			}
		}

		public ObjectResponse updateClinic(ClinicModel clinic)
		{
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();
					String query = $@" select * from Ecli_info where pk_seq = '{clinic.id}'";

					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"id này không tồn tại trong hệ thống",
							content = clinic
						};
					}

					query = $@" select * from Ecli_info where pk_seq <> '{clinic.id}' and ma='{clinic.maClinic}'";
					dtc = db.getDataTable(query);
					if (dtc.Rows.Count > 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Mã này đã tồn tại trong hệ thống",
							content = clinic
						};
					}


					query = $@" UPDATE [dbo].[Ecli_info]
								   SET [ma] = N'{clinic.maClinic}'
									  ,[name] = N'{clinic.name}'
									  ,[shortName] = N'{clinic.shortName}'
									  ,[address] = N'{clinic.address}'
									  ,[short_description] = N'{clinic.short_description}'
									  ,[short_description_html] = N'{clinic.short_description_html}'
									  ,[description] = N'{clinic.description}'
									  ,[description_html] = N'{clinic.description_html}'
									  ,[hotline] = '{clinic.hotline}'
									  ,[updatedAt] = GETDATE()
									  ,[image] = '{clinic.image}'
								 WHERE pk_seq='{clinic.id}'
                            ";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi cập nhật phòng khám {clinic.id}-{clinic.name}",
							content = clinic
						};
					}

					query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, N'Hoạt động' trangthai	,shortName	,address,short_description,	short_description_html	,description,	description_html	,hotline,image	 from Ecli_info where pk_seq={clinic.id}";
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
					content = clinic
				};
			}
		}
		public ObjectResponse getListClinic()
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, case when trangthai=0 then N'Chưa xác nhận' when trangthai=1 then N'Hoạt động' when trangthai=2 then N'Đã xóa' else '' end as trangthai	,shortName	,address,short_description,	short_description_html	,description,	description_html	,hotline,image	 from Ecli_info ";
					DataTable dt = db.getDataTable(query);
					db.CLose_Connection();
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
					content = ""
				};
            }
        }
        public ObjectResponse getDetailClinic(String id)
        {
            try
            {
                using (var db = new clsDB())
                {
                    String query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, case when trangthai=0 then N'Chưa xác nhận' when trangthai=1 then N'Hoạt động' when trangthai=2 then N'Đã xóa' else '' end as trangthai	,shortName	,address,short_description,	short_description_html	,description,	description_html	,hotline,image	 from Ecli_info where pk_seq={id}";
                    DataTable dt = db.getDataTable(query);
                    db.CLose_Connection();
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
                    content = ""
                };
            }
        }
		public ObjectResponse deleteClinic(String id)
		{
			try
			{
				using (var db = new clsDB())
				{
					db.BeginTran();
					String query = $@" delete Ecli_info where pk_seq = '{id}'";

					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi xóa phòng khám",
							content = ""
						};
					}

					db.CommitAndDispose();
					return new ObjectResponse
					{
						result = 1,
						message = "Xóa phòng khám thành công",
					};

				}
			}
			catch (Exception e)
			{
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = ""
				};
			}
		}
		public ObjectResponse getListClinicbyKeyword(String keyword)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, case when trangthai=0 then N'Chưa xác nhận' when trangthai=1 then N'Hoạt động' when trangthai=2 then N'Đã xóa' else '' end as trangthai	,shortName	,address,short_description,	short_description_html	,description,	description_html	,hotline,image	 
										from Ecli_info where '{keyword}'='0'  or cast(pk_seq as varchar(20)) +isnull(ma,'') + isnull(name,'') like N'%{keyword}%'";
					DataTable dt = db.getDataTable(query);
					db.CLose_Connection();
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
					content = ""
				};
			}
		}
	}
}