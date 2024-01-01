using OneSolution_API.Managers;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.kl
{
    public class SpecialtyModel
    {
        public String id { get; set; }
        public String ma { get; set; }
        public String trangthaiid { get; set; }
        public String name { get; set; }
        public String short_description { get; set; }
        public String short_description_html { get; set; }
        public String description { get; set; }
        public String description_html { get; set; }
        public String image { get; set; }

		public ObjectResponse createSpe(SpecialtyModel spe)
		{
			try
			{

				using (var db = new clsDB())
				{
					db.BeginTran();
					
					String query = $@" select * from Espe_info where ma = '{spe.ma}'";


					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count > 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Mã này đã tồn tại trong hệ thống",
							content = spe
						};
					}


					query = $@" insert into Espe_info(ma,	name,	trangthai,short_description,	short_description_html	,description,	description_html	,image	,createdAt)
                                select N'{spe.ma}',N'{spe.name}',1,N'{spe.short_description}',N'{spe.short_description_html}',N'{spe.description}',N'{spe.description_html}','{spe.image}',GETDATE()
                            ";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi tạo phòng khám {spe.name}",
							content = spe
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
							message = $@"L2: Lỗi khi tạo phòng khám {spe.name}",
							content = spe
						};
					}


					query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, N'Hoạt động' trangthai,short_description,	short_description_html	,description,	description_html,image from Espe_info where pk_seq={id}";
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
					content = spe
				};
			}
		}

		public ObjectResponse updateSpe(SpecialtyModel spe)
		{
			try
			{
				if(String.IsNullOrEmpty(spe.ma) || spe.ma.Length <= 0 )
                {
					return new ObjectResponse
					{
						result = 0,
						message = $@"Chưa nhập mã vui lòng nhập mã",
						content = spe
					};
				}
				if (String.IsNullOrEmpty(spe.name) || spe.name.Length <= 0)
				{
					return new ObjectResponse
					{
						result = 0,
						message = $@"Chưa nhập tên chuyên khoa vui lòng nhập tên chuyên khoa",
						content = spe
					};
				}
				using (var db = new clsDB())
				{
					db.BeginTran();
					String query = $@" select * from Espe_info where pk_seq = '{spe.id}'";

					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"id này không tồn tại trong hệ thống",
							content = spe
						};
					}

					query = $@" select * from Espe_info where pk_seq <> '{spe.id}' and ma='{spe.ma}'";
					dtc = db.getDataTable(query);
					if (dtc.Rows.Count > 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Mã này đã tồn tại trong hệ thống",
							content = spe
						};
					}


					query = $@" UPDATE [dbo].[Espe_info]
								   SET [ma] = N'{spe.ma}'
									  ,[name] = N'{spe.name}'
									  ,[short_description] = N'{spe.short_description}'
									  ,[short_description_html] = N'{spe.short_description_html}'
									  ,[description] = N'{spe.description}'
									  ,[description_html] = N'{spe.description_html}'
									  ,[updatedAt] = GETDATE()
									  ,[image] = '{spe.image}'
								 WHERE pk_seq='{spe.id}'
                            ";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi cập nhật chuyên khoa {spe.id}-{spe.name}",
							content = spe
						};
					}

					query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, N'Hoạt động' trangthai,short_description,	short_description_html	,description,	description_html,image from Espe_info where pk_seq={spe.id}";
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
					content = spe
				};
			}
		}
		public ObjectResponse getListSpe()
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, N'Hoạt động' trangthai,short_description,	short_description_html	,description,	description_html,image from Espe_info ";
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
		public ObjectResponse getDetailSpe(String id)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, N'Hoạt động' trangthai,short_description,	short_description_html	,description,	description_html,image from Espe_info  where pk_seq={id} ";
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
		public ObjectResponse deleteSpecialtyById(String id)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"delete  Espe_info  where pk_seq={id} ";

                    if (!db.updateQuery(query))
                    {
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = "Lỗi khi xóa user",
						};
					}
					db.CommitAndDispose();
					return new ObjectResponse
					{
						result = 1,
						message = "Success",
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
		public ObjectResponse getListSpebyKeyword(String keyword)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,ma,	name,	trangthai as trangthaiid, N'Hoạt động' trangthai,short_description,	short_description_html	,description,	description_html,image from Espe_info where ('{keyword}'='0' or cast(pk_seq as varchar(20)) +isnull(ma,'')+isnull(name,'') like N'%{keyword}%' ) ";
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