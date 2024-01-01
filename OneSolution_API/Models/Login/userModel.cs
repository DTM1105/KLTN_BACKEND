using Newtonsoft.Json.Linq;
using OneSolution_API.Managers;
using OneSolution_API.Managers.ModelsToken;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.Login
{
    public class userModel
    {
        public string id { get; set; }
		public string name { get; set; }
		public string ngaysinh { get; set; }
		public string email { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string trangthai { get; set; }
		public string gender { get; set; }
		public string roleID { get; set; }
		public string phonenumber { get; set; }
		public string avtimage { get; set; }
		public string maxacnhan { get; set; }
		public string address { get; set; }
		public string clinicId { get; set; }
		public static Boolean isValid(String value)
        {
            if (String.IsNullOrEmpty(value))
            {
				return false;
            }
			return true;
        }

		public ObjectResponse register(userModel userModel)
		{
			try
			{
				using (var db = new clsDB())
				{
					Random random = new Random();
					int randomNumber = random.Next(1000, 9999);

					string genderText = userModel.gender == null ? "null" : userModel.gender;
					db.BeginTran();
					String query = $@" select * from Euser where username='{userModel.username}' or email = '{userModel.email}'";


					DataTable dtc = db.getDataTable(query);
					if(dtc.Rows.Count > 0)
                    {
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"username, email này đã tồn tại trong hệ thống",
							content = userModel
						};
					}


					query = $@" insert into Euser(name,ngaysinh,email,username,password,trangthai,gender,roleID,phonenumber,avtimage,createdAt,diem,maxacnhan,address)
                                select N'{userModel.name}','{userModel.ngaysinh}','{userModel.email}','{userModel.username}',PWDENCRYPT('{userModel.password}'),0,{genderText},1
                                        ,'{userModel.phonenumber}','{userModel.avtimage}',GETDATE(),0,{randomNumber},N'{userModel.address}'
                            ";
					

					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi tạo user {userModel.username}",
							content = userModel
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
							message = $@"L2: Lỗi khi tạo user {userModel.username}",
							content = userModel
						};
					}
                    if (!MailModel.sendMail(userModel, randomNumber))
                    {
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L3: Lỗi khi gửi mail đến  {userModel.email}, vui lòng kiểm tra lại",
							content = userModel
						};
					}
					
					query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Chưa xác nhận' as trangthai,case when gender = 1 then N'Nam' when gender = 0 then 'Nữ' else '' end as  gender
									,roleID, phonenumber, avtimage,address from Euser where pk_seq={id}";
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
			catch(Exception e)
            {
				return new ObjectResponse
				{
					result = 0,
					message = "Exception:" + e.Message,
					content = userModel
				};
			}
		}
		public ObjectResponse verifyRegister(userModel userModel)
		{
			try
			{
				using (var db = new clsDB())
				{
					
					db.BeginTran();


					String query = $@" select * from Euser where username='{userModel.username}' and email = '{userModel.email}'";

					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"username, email này không có trong hệ thống vui lòng kiểm tra lại",
							content = new
							{
								username = userModel.username,
								email = userModel.email,
								maxacnhan = userModel.maxacnhan
							}
						};
					}
					query = $@" select pk_seq from Euser where username='{userModel.username}' and maxacnhan = {maxacnhan}";

					int check = db.getFirsIntValueSqlCatchException(query);
					if (check <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"Mã xác nhận sai!!!! vui lòng nhập lại",
							content = new
                            {
								username = userModel.username,
								email = userModel.email,
								maxacnhan= userModel.maxacnhan
							}
						};
					}

					query = $"update Euser set trangthai=1 where pk_seq={check}";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi cập nhật user {userModel.username}",
							content = new
							{
								username = userModel.username,
								email = userModel.email,
								maxacnhan = userModel.maxacnhan
							}
						};
					}

					query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Đã xác nhận' as trangthai,case when gender = 1 then N'Nam' when gender = 0 then 'Nữ' else '' end as  gender
									,roleID, phonenumber, avtimage from Euser where pk_seq={check}";
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
					content = new
					{
						username = userModel.username,
						email = userModel.email,
						maxacnhan = userModel.maxacnhan
					}
				};
			}
		}
		public ObjectResponse login(userModel userModel)
		{
			try
			{
				using (var db = new clsDB())
				{

					String query = $@" select * from Euser where username='{userModel.username}' and trangthai =1";

					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = $@"username này không có trong hệ thống vui lòng kiểm tra lại",
							content = new
							{
								username = userModel.username,
								password = userModel.password,
							}
						};
					}
					query = $@" select pk_seq from Euser where username='{userModel.username}' and  PWDCOMPARE('" + userModel.password + "',password) = 1 ";

					int check = db.getFirsIntValueSqlCatchException(query);
					if (check <= 0)
					{
						return new ObjectResponse
						{
							result = 0,
							message = $@"Sai mật khẩu",
							content = new
							{
								username = userModel.username,
								password = userModel.password,
							}
						};
					}

					query = $@"select pk_seq as id,name,email,username,trangthai as trangthaiId,roleID from Euser where pk_seq={check}";

/*					query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Hoạt động' as trangthai,case when gender = 1 then N'Nam' when gender = 0 then 'Nữ' else '' end as  gender
									,roleID, phonenumber, avtimage from Euser where pk_seq={check}";*/
					DataTable dt = db.getDataTable(query);
					String account = XuLy.ParseDataRowToJson(dt.Rows[0]);
					TokenModel guest = JObject.Parse(account).ToObject<TokenModel>();
					IAuthContainerModel model = Signature.GetJWTContainerModel(guest.id.ToString(),guest.roleid.ToString());
					IAuthService authService = new JWTService(model.SecretKey);
					string token = authService.GenerateToken(model);

					query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Hoạt động' as trangthai,case when gender = 1 then N'Nam' when gender = 0 then 'Nữ' else '' end as  gender
									,roleID, phonenumber, avtimage from Euser where pk_seq={check}";
					dt = db.getDataTable(query);
					dt.Columns.Add("authorization", typeof(String));
					dt.Rows[0]["authorization"] = token;
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
					content = new
					{
						username = userModel.username,
						email = userModel.email,
						maxacnhan = userModel.maxacnhan
					}
				};
			}
		}
		public ObjectResponse getListPatient()
        {
            try
            {
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Đã xác nhận' as trangthai,case when gender = 1 then N'Nam' when gender = 0 then 'Nữ' else '' end as  gender
									,roleID, phonenumber, avtimage from Euser where trangthai=1 and roldeId=1";
					DataTable dt = db.getDataTable(query);
					if(dt.Rows.Count <= 0)
                    {
						return new ObjectResponse
						{
							result = 1,
							message = "Không có dữ liệu",
						};
					}
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
					message = "Lỗiiiiiii",
				};
			}
        }
		public ObjectResponse createUser(userModel userModel)
		{
			try
			{
				using (var db = new clsDB())
				{
					string genderText = userModel.gender == null ? "null" : userModel.gender;
					string clinic =  String.IsNullOrEmpty(userModel.clinicId) ? "null" : userModel.clinicId;
					db.BeginTran();
					String query = $@" select * from Euser where username='{userModel.username}' or email = '{userModel.email}'";


					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count > 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"username, email này đã tồn tại trong hệ thống",
							content = userModel
						};
					}


					query = $@" insert into Euser(name,ngaysinh,email,username,password,trangthai,gender,roleID,phonenumber,avtimage,createdAt,diem,clinicId)
                                select N'{userModel.name}','{userModel.ngaysinh}','{userModel.email}','{userModel.username}',PWDENCRYPT('{userModel.password}'),1,{genderText},{userModel.roleID}
                                        ,'{userModel.phonenumber}','{userModel.avtimage}',GETDATE(),0,{clinic}
                            ";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi tạo user {userModel.username}",
							content = userModel
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
							message = $@"L2: Lỗi khi tạo user {userModel.username}",
							content = userModel
						};
					}

					query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Hoạt động' as trangthai,case when gender = 1 then N'Nam' when gender = 0 then 'Nữ' else '' end as  gender
									,roleID, phonenumber, avtimage from Euser where pk_seq={id}";
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
					content = userModel
				};
			}
		}
		public ObjectResponse updateUser(userModel userModel)
		{
			try
			{
				using (var db = new clsDB())
				{
					string genderText = userModel.gender == null ? "null" : userModel.gender;
					db.BeginTran();

					String query = $@" select * from Euser where pk_seq='{userModel.id}'";


					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"user này không có trong hệ thống",
							content = userModel
						};
					}

					query = $@" select * from Euser where pk_seq <> '{userModel.id}' and (email = '{userModel.email}')";

					dtc = db.getDataTable(query);
					if (dtc.Rows.Count > 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"email này đã tồn tại trong hệ thống",
							content = userModel
						};
					}
					String clinincText = "null";
					if(!String.IsNullOrEmpty(userModel.clinicId))
                    {
						clinincText = userModel.clinicId.Trim();

					}

					query = $@" UPDATE [dbo].[Euser]
							   SET [name] = N'{userModel.name}'
								  ,[ngaysinh] = '{userModel.ngaysinh}'
								  ,[email] = '{userModel.email}'
								  ,[gender] = '{userModel.gender}'
								  ,[roleID] = '{userModel.roleID}'
								  ,[phonenumber] = '{userModel.phonenumber}'
								  ,[avtimage] = '{userModel.avtimage}'
								  ,[address]=N'{userModel.address}'
								  ,[updatedAt] = GETDATE() 
								  ,[clinicId]={clinincText}
							 WHERE pk_seq={userModel.id}
                            ";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi tạo user {userModel.username}",
							content = userModel
						};
					}

					query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Hoạt động' as trangthai,gender
									,roleID, phonenumber, avtimage,address from Euser where pk_seq={userModel.id}";
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
					content = userModel
				};
			}
		}
		public ObjectResponse getListUser(String userId)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId
									,case when trangthai = 1 then N'Hoạt động' when trangthai=2 then N'Đã xóa' else N'Chưa xác nhận' end as trangthai
									,gender
									,roleID
									,case when roleID=0 then N'Admin' when roleID=1 then N'Bệnh nhân' when roleID=2 then N'Bác sĩ' when roleID=3 then N'Nhân viên phòng khám' end as phanquyen
									, phonenumber, avtimage,address,clinicId 
							from Euser where ('{userId}'='0' or username+''+email+''+name+'-'+cast(pk_seq as nvarchar(20))like N'%{userId}%') order by pk_seq";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 1,
							message = "Không có dữ liệu",
						};
					}
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
					message = "Lỗiiiiiii",
				};
			}
		}
		public ObjectResponse deleteUser(String userId)
		{
			try
			{
				using (var db = new clsDB())
				{
					String query = $@" select * from Euser where pk_seq='{userId}'";

					db.BeginTran();
					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"userid {userId} này không có trong hệ thống",
							content = null
						};
					}

					query = $" update Euser set trangthai =2 where pk_seq ='{userId}'";
					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi xóa user {userId}",
							content = null
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
					message = "Lỗiiiiiii",
				};
			}
		}
		public ObjectResponse getProfile(String userId)
		{
			try
			{
				
				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId
									,case when trangthai = 1 then N'Hoạt động' when trangthai=2 then N'Đã xóa' else N'Chưa xác nhận' end as trangthai
									, gender
									,roleID
									,case when roleID=0 then N'Admin' when roleID=1 then N'Bệnh nhân' when roleID=2 then N'Bác sĩ' when roleID=3 then N'Nhân viên phòng khám' end as phanquyen
									, phonenumber, avtimage,address,b.short_description,b.description from Euser a
									left join Edt_info b on a.pk_seq=b.userId
									where {userId}=0 or pk_seq={userId}";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 1,
							message = "Không có dữ liệu",
						};
					}
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
					message = "Lỗiiiiiii",
				};
			}
		}
		public ObjectResponse getSearchHomePage(String keyword)
		{
			try
			{

				using (var db = new clsDB())
				{
					String query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId
									,case when trangthai = 1 then N'Hoạt động' when trangthai=2 then N'Đã xóa' else N'Chưa xác nhận' end as trangthai
									,case when gender = 1 then N'Nam' when gender = 0 then 'Nữ' else '' end as  gender
									,roleID
									,case when roleID=0 then N'Admin' when roleID=1 then N'Bệnh nhân' when roleID=2 then N'Bác sĩ' when roleID=3 then N'Nhân viên phòng khám' end as phanquyen
									, phonenumber, avtimage,address from Euser ";
					DataTable dt = db.getDataTable(query);
					if (dt.Rows.Count <= 0)
					{
						return new ObjectResponse
						{
							result = 1,
							message = "Không có dữ liệu",
						};
					}
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
					message = "Lỗiiiiiii",
				};
			}
		}
		public ObjectResponse updateProfile(userModel userModel)
		{
			try
			{
				using (var db = new clsDB())
				{
					string genderText = userModel.gender == null ? "null" : userModel.gender;
					db.BeginTran();

					String query = $@" select * from Euser where pk_seq='{userModel.id}'";


					DataTable dtc = db.getDataTable(query);
					if (dtc.Rows.Count <= 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"user này không có trong hệ thống",
							content = userModel
						};
					}

					query = $@" select * from Euser where pk_seq <> '{userModel.id}' and (email = '{userModel.email}')";

					dtc = db.getDataTable(query);
					if (dtc.Rows.Count > 0)
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"email này đã tồn tại trong hệ thống",
							content = userModel
						};
					}
					String clinincText = "null";
					if (!String.IsNullOrEmpty(userModel.clinicId))
					{
						clinincText = userModel.clinicId.Trim();

					}

					query = $@" UPDATE [dbo].[Euser]
							   SET [name] = N'{userModel.name}'
								  ,[ngaysinh] = convert(varchar(10),'{userModel.ngaysinh}')
								  ,[email] = '{userModel.email}'
								  ,[gender] = '{userModel.gender}'
								  ,[phonenumber] = '{userModel.phonenumber}'
								  ,[avtimage] = '{userModel.avtimage}'
								  ,[address]=N'{userModel.address}'
								  ,[updatedAt] = GETDATE() 
							 WHERE pk_seq={userModel.id}
                            ";


					if (!db.updateQuery(query))
					{
						db.RollbackAndDispose();
						return new ObjectResponse
						{
							result = 0,
							message = $@"L1: Lỗi khi chỉnh sửa {userModel.username}",
							content = userModel
						};
					}

					query = $@"select pk_seq as id,name,ngaysinh,email,username,trangthai as trangthaiId,N'Hoạt động' as trangthai,gender
									,roleID, phonenumber, avtimage,address from Euser where pk_seq={userModel.id}";
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
					content = userModel
				};
			}
		}
	}
}