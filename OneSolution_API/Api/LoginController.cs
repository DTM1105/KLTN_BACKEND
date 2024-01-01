using OneSolution_API.Managers;
using OneSolution_API.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OneSolution_API.Api
{
    public class LoginController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Login(String username, String password)
        {
            if (username == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập username",
                });
            }
            if (password == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Vui lòng nhập password",
                });
            }

            try
            {
                using (var db = new clsDB())
                {
                    String query = " select pk_Seq  from nhanvien where trangthai = 1 and dangnhap = '" + username + "' and PWDCOMPARE('" + password + "',MATKHAU) = 1 ";
                    string id = db.getFirstStringValueSqlCatchException(query);
                    if (id == null || id.Length <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                        {
                            result = 0,
                            message = "Tên đăng nhập hoặc tài khoản không đúng. Vui lòng thử lại!"
                        });
                    }
                    query = "select isnull(phanloai,0) phanloai from nhanvien where pk_seq=" + id;
                    DataTable dt = db.getDataTable(query);
                    int phanloai = 0;
                    String tennhanvien = "";
                    String userid = "";
                    String congty_fk = "";
                    String npp_fk = "";
                    int sochinhanh = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        phanloai = int.Parse(row["phanloai"].ToString());
                    }
                    query = $@"select a.pk_seq, a.ten as tennhanvien, isnull(loai, '') as loai, phanloai, a.congty_fk, a.NPP_FK
                                , isnull(npp.ten,'') nppten, ct.ten as tencongty, 
                                isnull( ( select count(*) from NHANVIEN_NHAPHANPHOI where nhanvien_fk = a.pk_seq ) , 0 ) as soChinhanh,	
                                ( select isCongty from NHAPHANPHOI where pk_seq = a.NPP_FK ) as isCongty 	
                                from nhanvien a inner join erp_congty ct on a.congty_fk = ct.pk_seq 
                                left join nhaphanphoi npp on a.npp_fk = npp.pk_seq and npp.iscongty = 0 
                                where dangnhap='{username}' and phanloai='{phanloai.ToString()}' and a.trangthai='1'";
                    dt = db.getDataTable(query);
                    foreach (DataRow row in dt.Rows)
                    {
                        tennhanvien = row["tennhanvien"].ToString();
                        congty_fk = row["congty_fk"].ToString();
                        sochinhanh = int.Parse(row["soChinhanh"].ToString());
                        npp_fk = row["NPP_FK"].ToString();
                    }
                    if (sochinhanh > 0)
                    {
                        query = $@"select cast(PK_SEQ as varchar(20)) as npp_fk, TEN as TenNPP 
                                     from NHAPHANPHOI where TRANGTHAI = '1' 
                                     and ( 	   PK_SEQ = ( select pk_seq from NHAPHANPHOI where congty_fk = '{congty_fk}' and isCongty = '1' ) 
	                                    or PK_SEQ in (  select Npp_fk from NHANVIEN_NHAPHANPHOI where nhanvien_fk = '{id.ToString()}' ) ) 
	                                    	";
                        dt = db.getDataTable(query);
                    }
                    else
                    {
                        query = $@"select cast(PK_SEQ as varchar(20)) as npp_fk, TEN as TenNPP 
                                     from NHAPHANPHOI where TRANGTHAI = '1' 
                                     and   pk_seq=" + npp_fk;
                        dt = db.getDataTable(query);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                    {
                        result = 1,
                        message = "Success!!!!",
                        content = new
                        {
                            userid = id,
                            tennhanvien = tennhanvien,
                            congty_fk = congty_fk,
                            nhaphanphoi = dt
                        }
                    });

                }

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Oops Lỗi rồi !!!!!" + e.Message,
                });
            }

        }
        [HttpGet]
        public HttpResponseMessage GetListChungTuCanDuyet(int npp_fk, int congty_fk, String userId)
        {
            try 
            {
                if (npp_fk == null)
                {
                    npp_fk = 0;
                }
                if (congty_fk == null)
                {
                    congty_fk = 0;
                }
                if (userId == null)
                {
                    userId = "0";
                }
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 1,
                    message = "Succcesss !!!!!" ,
                    content = new
                    {
                        muahang = GetDataMuaHangDuyet(npp_fk,congty_fk,userId),
                        congno= GetDataCongNoDuyet(npp_fk,congty_fk,userId),
                        sanxuat = GetDataLSXDuyet(npp_fk,congty_fk,userId),
                        banhang = GetDataBanHangDuyet(npp_fk, congty_fk, userId)
                    }

                });

            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ObjectResponse
                {
                    result = 0,
                    message = "Oops Lỗi rồi !!!!!" + e.Message,
                });
            }
        }
        public static int GetDataLSXDuyet(int npp_fk, int congty_fk, String userId)
        {
            try
            {
                using (var db = new clsDB())
                {

                    String query = $@"  SELECT count(1) as cnt
                                         FROM ERP_LENHSANXUAT_GIAY  A 
                                         WHERE 1=1  AND ISNULL(DUYET,'0') ='0' and A.TRANGTHAI='1' and A.CONGTY_FK = {congty_fk} ";

                    DataTable dt = db.getDataTable(query);
                    foreach (DataRow row in dt.Rows)
                    {
                        return int.Parse(row["cnt"].ToString());
                    }
                    return 0;

                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static int GetDataMuaHangDuyet(int npp_fk, int congty_fk, String userId)
        {
            try
            {
                using (var db = new clsDB())
                {

                    String query = $@" select sum(cnt) as cnt from  (
                                        SELECT count(1) as cnt
                                        FROM ERP_DENGHIMUAHANG MUAHANG     
                                        INNER JOIN NHANVIEN NV ON NV.PK_SEQ = MUAHANG.NGUOITAO       
                                        INNER JOIN ERP_DONVITHUCHIEN DVTH ON DVTH.PK_SEQ = MUAHANG.DONVITHUCHIEN_FK     
                                        inner JOIN      
                                        ( select distinct DENGHIMUAHANG_FK  from   ERP_DUYETDENGHIMUAHANG DUYETMUAHANG      
                                        LEFT JOIN ERP_CHUCDANH CHUCDANH ON CHUCDANH.PK_SEQ = DUYETMUAHANG.CHUCDANH_FK      
                                        where    CHUCDANH.NHANVIEN_FK = '{userId}' AND DUYETMUAHANG.TRANGTHAI = 0     
                                        ) DUYETMUAHANG ON DUYETMUAHANG.DENGHIMUAHANG_FK = MUAHANG.PK_SEQ       
                                        WHERE MUAHANG.TRANGTHAI NOT IN ('3','4')   AND isnull(MUAHANG.DACHOT, 0) = '1'   
                                        AND MUAHANG.PK_SEQ NOT IN(SELECT DENGHIMUAHANG_FK  FROM ERP_DUYETDENGHIMUAHANG WHERE TRANGTHAI=1 AND QUYETDINH=1)  
                                        AND MUAHANG.congty_fk =  '{congty_fk}'  
                                        and  MUAHANG.NPP_FK='{npp_fk}'  
                                        
                                        UNION ALL

                                        select count(1)
                                        from erp_muahang a  
                                        INNER JOIN NHANVIEN NV ON NV.PK_SEQ = a.NGUOITAO      
                                        INNER JOIN ERP_MUAHANG_SP MUAHANG_SP ON MUAHANG_SP.MUAHANG_FK = a.PK_SEQ      
                                        INNER JOIN ERP_DONVITHUCHIEN DVTH ON DVTH.PK_SEQ = a.DONVITHUCHIEN_FK    
                                        inner JOIN     
                                        ( select distinct MUAHANG_FK  from   ERP_DUYETMUAHANG DUYETMUAHANG     
                                        LEFT JOIN ERP_CHUCDANH CHUCDANH ON CHUCDANH.PK_SEQ = DUYETMUAHANG.CHUCDANH_FK     
                                        where    CHUCDANH.NHANVIEN_FK = '{userId}' AND DUYETMUAHANG.TRANGTHAI = 0    
                                        ) DUYETMUAHANG    
                                        ON DUYETMUAHANG.MUAHANG_FK = a.PK_SEQ      

                                        where  1=1
                                            AND a.TRANGTHAI NOT IN ('3','4')   AND isnull(a.DACHOT, 0) = '1'  
 	                                        AND a.PK_SEQ NOT IN(SELECT MUAHANG_FK  FROM ERP_DUYETMUAHANG WHERE TRANGTHAI=1 AND QUYETDINH=1) 
 	                                        AND a.congty_fk =  '{congty_fk}' and a.LOAI =  0
                                            and a.DONVITHUCHIEN_FK in (select dvth_fk from NHANVIEN_QUYENPB where nhanvien_fk='{userId}') and a.NPP_FK= '{npp_fk}'

                                        UNION ALL

                                        select  count(1)
                                        from erp_muahang a  
                                        INNER JOIN NHANVIEN NV ON NV.PK_SEQ = a.NGUOITAO      
                                        INNER JOIN ERP_MUAHANG_SP MUAHANG_SP ON MUAHANG_SP.MUAHANG_FK = a.PK_SEQ      
                                        INNER JOIN ERP_DONVITHUCHIEN DVTH ON DVTH.PK_SEQ = a.DONVITHUCHIEN_FK    
                                        inner JOIN     
                                        ( select distinct MUAHANG_FK  from   ERP_DUYETMUAHANG DUYETMUAHANG     
                                        LEFT JOIN ERP_CHUCDANH CHUCDANH ON CHUCDANH.PK_SEQ = DUYETMUAHANG.CHUCDANH_FK     
                                        where    CHUCDANH.NHANVIEN_FK = '{userId}' AND DUYETMUAHANG.TRANGTHAI = 0    
                                        ) DUYETMUAHANG    
                                        ON DUYETMUAHANG.MUAHANG_FK = a.PK_SEQ      
                                        where  1=1
                                            AND a.TRANGTHAI NOT IN ('3','4')   AND isnull(a.DACHOT, 0) = '1'  
 	                                        AND a.PK_SEQ NOT IN(SELECT MUAHANG_FK  FROM ERP_DUYETMUAHANG WHERE TRANGTHAI=1 AND QUYETDINH=1) 
 	                                        AND a.congty_fk =  '{congty_fk}' and a.LOAI =  1 
                                            and a.DONVITHUCHIEN_FK in (select dvth_fk from NHANVIEN_QUYENPB where nhanvien_fk='{userId}') and a.NPP_FK= '{npp_fk}'

                                        UNION ALL

                                         select  count(1)
                                         from erp_hopdongmuahang a  
                                         where  a.congty_fk = '{congty_fk}' AND  A.NPP_FK={npp_fk} and a.TRANGTHAI=3 
                                        ) data
                                        ";
                    DataTable dt = db.getDataTable(query);
                    foreach(DataRow row in dt.Rows)
                    {
                        return int.Parse(row["cnt"].ToString());
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static int GetDataCongNoDuyet(int npp_fk, int congty_fk, String userId)
        {
            try
            {
                using (var db = new clsDB())
                {

                    String query = $@"  SELECT N'Đề nghị tạm ứng' as loaichungtu,ISNULL(TAMUNG.ISTP,0) AS ISTP
	                                                            ,ISNULL(TAMUNG.ISKTV, 0) ISKTV, ISNULL(TAMUNG.ISKTT, 0) ISKTT, ISNULL(TAMUNG.ISDACHOT,0) ISDACHOT
	                                                            ,ISNULL((select SUM(ISTP) from ERP_CHUCDANH where NHANVIEN_FK={userId} and DVTH_FK = (SELECT PHONGBAN_FK FROM NHANVIEN WHERE PK_SEQ = NV.PK_SEQ) AND isTP = 1  ),0) as quyentp
	                                                            ,ISNULL((select SUM(isKTV) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isKTV=1),0) as quyenkt
	                                                            ,ISNULL((select SUM(isKTV) from ERP_CHUCDANH CD left join ERP_CHUCDANH_NHANVIEN cdnv on cd.PK_SEQ=cdnv.CHUCDANH_FK where cdnv.NHANVIEN_FK={userId} 
																												                                                            and CD.isKTV=1 and CD.congty_fk={congty_fk} 
																												                                                            and CD.trangthai=1),0) as quyennvkt 
                                                                ,ISNULL(TAMUNG.TRANGTHAI,0)TRANGTHAI
	                                                            ,ISNULL((select SUM(isKTT) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isKTT=1),0) as quyenGD 
                                                                ,0 as lanhdaokyduyet, 0 as isLD, 0 as quyenLD
                                                        FROM ERP_TAMUNG TAMUNG 
                                                        INNER JOIN NHANVIEN NV ON NV.PK_SEQ = TAMUNG.NGUOITAO  
                                                        WHERE TAMUNG.TRANGTHAI IN ( 0 , 1 ) AND isnull(TAMUNG.ISDACHOT, 0) = '1' 
                                                                AND TAMUNG.NPP_FK = '{npp_fk}' 
                                                                AND TAMUNG.donvithuchien_fk in (SELECT DVTH_FK FROM NHANVIEN_QUYENPB where NHANVIEN_FK={userId} ) 
                                                                AND ISNULL( TAMUNG.ISHOANTAT, 0 ) = 0  
                                                               -- AND  	(  {userId} in  ( SELECT NHANVIEN_FK FROM NHANVIEN_QUYENPB WHERE  DVTH_FK =nv.PHONGBAN_FK  AND {userId} IN (SELECT NHANVIEN_FK FROM ERP_CHUCDANH WHERE ISNULL(ISTP,0)=1) and ISNULL(TAMUNG.iscs,0)=1  ) 
                                                               --          OR {userId} = NV.PK_SEQ  
                                                               --          OR ({userId} in  ( SELECT NHANVIEN_FK FROM ERP_CHUCDANH WHERE ISKTT=1 AND ISNULL(TAMUNG.ISTP,0)=1  and trangthai=1 )  ) 
                                                               --          OR ( {userId} in  ( SELECT NHANVIEN_FK FROM ERP_CHUCDANH WHERE ISKTV=1 AND ISNULL(TAMUNG.ISTP,0)=1  and trangthai=1 )  )
                                                               --          OR ({userId} in  ( SELECT CDNV.NHANVIEN_FK FROM ERP_CHUCDANH  CD 									
                                                               --                                                     LEFT JOIN ERP_CHUCDANH_NHANVIEN CDNV ON CD.PK_SEQ=CDNV.CHUCDANH_FK 
                                                               --                                                     WHERE CD.ISKTV=1 AND ISNULL(TAMUNG.ISTP,0)=1 and CD.trangthai=1 )  
                                                               --         ))
                                            UNION ALL
                                                        SELECT N'Đề nghị thanh toán' as loaichungtu,ISNULL(MUAHANG.ISTP,0) ISTP, ISNULL(MUAHANG.ISKTV, 0) ISKTV, ISNULL(MUAHANG.ISKTT, 0) ISKTT
	                                                        , ISNULL(MUAHANG.ISDACHOT,0) ISDACHOT
	                                                        , ISNULL((select SUM(ISTP) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isTP=1 and congty_fk={congty_fk} and DVTH_FK=MUAHANG.DONVITHUCHIEN_FK),0) as quyentp
	                                                        , ISNULL((select SUM(isKTV) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isKTV=1 and congty_fk={congty_fk}),0) as quyenkt
	                                                        , ISNULL((select SUM(isKTV) from ERP_CHUCDANH CD left join ERP_CHUCDANH_NHANVIEN cdnv on cd.PK_SEQ=cdnv.CHUCDANH_FK where cdnv.NHANVIEN_FK={userId} and CD.isKTV=1 
																											                                                        and CD.congty_fk={congty_fk} and CD.trangthai=1),0) as quyennvkt
	                                                        , MUAHANG.TRANGTHAI TRANGTHAI
	                                                        , ISNULL((select SUM(isKTT) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isKTT=1 and congty_fk={congty_fk}),0) as quyenGD
	                                                        , ISNULL(muahang.ISLANHDAOKYDUYET,0) as lanhdaokyduyet
	                                                        , ISNULL(MUAHANG.IsLD, 0) as isLD
	                                                        , ISNULL((select SUM(islanhdao) from ERP_CHUCDANH where NHANVIEN_FK={userId} and islanhdao=1 and congty_fk={congty_fk}),0) as quyenLD

                                                         FROM ERP_MUAHANG MUAHANG 
                                                         INNER JOIN NHANVIEN NV ON NV.PK_SEQ = MUAHANG.NGUOITAO 
                                                         WHERE isnull(MUAHANG.ISDACHOT, 0) = '1' 
                                                         AND MUAHANG.ISDNTT = 1
                                                         AND MUAHANG.NPP_FK= '{npp_fk}'  
                                                         and MUAHANG.donvithuchien_fk in (SELECT DVTH_FK FROM NHANVIEN_QUYENPB where NHANVIEN_FK={userId} ) 
                                                         and MUAHANG.TYPE = '1' 
                                                        -- AND (  {userId} in  ( SELECT NHANVIEN_FK FROM NHANVIEN_QUYENPB WHERE  DVTH_FK=MUAHANG.DONVITHUCHIEN_FK 
														--	                                                        AND {userId} IN (SELECT NHANVIEN_FK FROM ERP_CHUCDANH WHERE ISNULL(ISTP,0)=1) and ISNULL(muahang.iscs,0)=1 )
 		                                                --        OR {userId} = NV.PK_SEQ  
 		                                                --        OR {userId} = MUAHANG.TRUONGPHONG_FK  
 		                                                --        OR {userId} = MUAHANG.KETOANVIEN_FK  
 		                                                --        OR {userId} = MUAHANG.KETOANTRUONG_FK  
  		                                                --        OR ({userId} in  (SELECT NHANVIEN_FK FROM ERP_CHUCDANH WHERE ISKTT=1 and ISNULL(muahang.isktv,0)=1  ) and {userId} IN (SELECT NHANVIEN_fK FROM NHANVIEN_QUYENPB)) 
		                                                --        OR ({userId} in  (SELECT NHANVIEN_FK FROM ERP_CHUCDANH WHERE ISLD=1 AND MUAHANG.ISLANHDAOKYDUYET=1 ) and {userId} IN (SELECT NHANVIEN_fK FROM NHANVIEN_QUYENPB)) 
		                                                --        OR ({userId} in  (SELECT NHANVIEN_FK FROM ERP_CHUCDANH WHERE ISKTV=1 and ISNULL(muahang.istp,0)=1   )and {userId} IN (SELECT NHANVIEN_fK FROM NHANVIEN_QUYENPB)) 
		                                                --        OR ({userId} in  (SELECT CDNV.NHANVIEN_FK FROM ERP_CHUCDANH  CD  LEFT JOIN ERP_CHUCDANH_NHANVIEN CDNV ON CD.PK_SEQ=CDNV.CHUCDANH_FK
 								  	                    --                                    WHERE CD.ISKTV=1 AND ISNULL(MUAHANG.ISTP,0)=1) 
											            --                                            and {userId} IN (SELECT NHANVIEN_fK FROM NHANVIEN_QUYENPB)) 
                                                        --  )

                                        UNION ALL
                                                       SELECT '',1 as isTP,isnull(TTHD.ISKTV,0) isktv,isnull(TTHD.ISKTTDUYET,0) as KTT
				                                                ,1 as ISDACHOT
			                                                , ISNULL((select SUM(ISTP) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isTP=1 and congty_fk={congty_fk} and DVTH_FK=TTHD.DVTH_FK),0) as quyentp
	                                                        , ISNULL((select SUM(isKTV) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isKTV=1 and congty_fk={congty_fk}),0) as quyenkt
	                                                        , ISNULL((select SUM(isKTV) from ERP_CHUCDANH CD left join ERP_CHUCDANH_NHANVIEN cdnv on cd.PK_SEQ=cdnv.CHUCDANH_FK where cdnv.NHANVIEN_FK={userId} and CD.isKTV=1  and CD.congty_fk={congty_fk} and CD.trangthai=1),0) as quyennvkt
	                                                        , TTHD.TRANGTHAI TRANGTHAI
	                                                        , ISNULL((select SUM(isKTT) from ERP_CHUCDANH where NHANVIEN_FK={userId} and isKTT=1 and congty_fk={congty_fk}),0) as quyenGD
	                                                        , 1 as lanhdaokyduyet
	                                                        , 1 as isLD
	                                                        , ISNULL((select SUM(islanhdao) from ERP_CHUCDANH where NHANVIEN_FK={userId} and islanhdao=1 and congty_fk={congty_fk}),0) as quyenLD
		                                                FROM ERP_THANHTOANHOADON TTHD 
		                                                LEFT JOIN ERP_NHACUNGCAP NCC ON TTHD.NCC_FK = NCC.PK_SEQ 
		                                                LEFT JOIN ERP_NHANVIEN NV ON TTHD.NHANVIEN_FK = NV.PK_SEQ 
		                                                LEFT JOIN ERP_KHACHHANG KH ON TTHD.KHACHHANG_FK = KH.PK_SEQ 
		                                                LEFT JOIN ERP_DONVITHUCHIEN DVTH ON TTHD.DVTH_FK = DVTH.PK_SEQ 		 
		                                                LEFT JOIN ERP_DOITUONGKHAC DTK ON DTK.PK_SEQ=TTHD.DOITUONGKHAC_FK
		                                                INNER JOIN NHANVIEN NV1 ON TTHD.NGUOITAO = NV1.PK_SEQ 
		                                                INNER JOIN ERP_TIENTE TT ON TTHD.TIENTE_FK = TT.PK_SEQ 
		                                                left JOIN NHANVIEN nc ON TTHD.NGUOICHOT = nc.PK_SEQ
		                                                left JOIN NHANVIEN ns ON TTHD.NGUOISUA = ns.PK_SEQ
		                                                left JOIN NHANVIEN nx ON TTHD.NGUOIXOA = ns.PK_SEQ
		                                                WHERE TTHD.TRANGTHAI = 0 AND ISNULL(TTHD.ISKTTDUYET,0) = 0 
                                                        AND 	(  {userId} in  ( 
		                                                        SELECT NHANVIEN_FK FROM ERP_CHUCDANH 
		                                                        WHERE NV.DVTH_FK = (SELECT PHONGBAN_FK FROM NHANVIEN WHERE PK_SEQ = NV.PK_SEQ)   AND ISTP=1  
		                                                )     
		                                                OR 	{userId} in  ( 
		                                                SELECT NHANVIEN_FK FROM ERP_CHUCDANH 
		                                                WHERE ISKTT=1  ) 
		                                                OR {userId} in  ( 
		                                                SELECT NHANVIEN_FK FROM ERP_CHUCDANH 
		                                                WHERE ISLANHDAO=1 AND TTHD.ISLANHDAOKYDUYET=1  ) 
		                                                OR {userId} in  ( 
		                                                SELECT NHANVIEN_FK FROM ERP_CHUCDANH 
		                                                WHERE ISKTV=1   ) 
		                                                )
		                                                AND TTHD.HTTT_FK IN (100000, 100001, 100003) and TTHD.CONGTY_FK = {congty_fk} AND TTHD.NPP_FK = {npp_fk} and ISNULL(TTHD.isktv,0)=1  
                                        ";

                    DataTable dt = db.getDataTable(query);
                    if(dt.Rows.Count <= 0)
                    {
                        return 0;
                    }

                    string TRANGTHAI = "", ISTP = "", ISKTV = "",
                                ISKTT = "", ISDACHOT = "", LANHDAOKYDUYET = "", ISLD = "";
                    double quyentp = 0, quyenkt = 0, quyennvkt = 0, quyenGD = 0, quyenLD = 0;
                    List<DataRow> dtR = new List<DataRow>();
                    String a = "";
                    //CHECK DATA TRONG ĐỀ NGHỊ TẠM ỨNG
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["loaichungtu"].ToString().Equals("Đề nghị tạm ứng"))
                        {
                            TRANGTHAI = row["TRANGTHAI"].ToString();
                            ISTP = row["ISTP"].ToString();
                            ISKTV = row["ISKTV"].ToString();
                            ISKTT = row["ISKTT"].ToString();
                            ISDACHOT = row["ISDACHOT"].ToString();
                            quyentp = Double.Parse(row["quyentp"].ToString());
                            quyenkt = Double.Parse(row["quyenkt"].ToString());
                            quyennvkt = Double.Parse(row["quyennvkt"].ToString());
                            quyenGD = Double.Parse(row["quyenGD"].ToString());

                            if (TRANGTHAI.Equals("0"))
                            {
                                if ((ISTP.Equals("0") && quyentp >= 1)
                               || (ISKTV.Equals("0") && (quyenkt >= 1 || quyennvkt >= 1))
                               || (ISKTT.Equals("0") && quyenGD >= 1))
                                {
                                    if (ISTP.Equals("0") && quyentp >= 1)
                                    {
                                        continue;
                                    }
                                    if (ISTP.Equals("1") && ISKTT.Equals("0") && quyenGD >= 1)
                                    {
                                        continue;
                                    }
                                }
                            }
                            dtR.Add(row);
                        }
                        else if (row["loaichungtu"].ToString().Equals("Đề nghị thanh toán"))
                        {
                            TRANGTHAI = row["TRANGTHAI"].ToString();
                            ISTP = row["ISTP"].ToString();
                            ISKTV = row["ISKTV"].ToString();
                            ISKTT = row["ISKTT"].ToString();
                            ISDACHOT = row["ISDACHOT"].ToString();
                            quyentp = Double.Parse(row["quyentp"].ToString());
                            quyenkt = Double.Parse(row["quyenkt"].ToString());
                            quyennvkt = Double.Parse(row["quyennvkt"].ToString());
                            quyenGD = Double.Parse(row["quyenGD"].ToString());
                            LANHDAOKYDUYET = row["lanhdaokyduyet"].ToString();
                            ISLD = row["isLD"].ToString();
                            quyenLD = Double.Parse(row["quyenLD"].ToString());

                            if (TRANGTHAI.Equals("0"))
                            {
                                if (ISTP.Equals("0") && quyentp >= 1 && !LANHDAOKYDUYET.Equals("1"))
                                {
                                    continue;
                                }
                                if (ISTP.Equals("1") && ISKTV.Equals("0") && (quyenkt >= 1) && !LANHDAOKYDUYET.Equals("1"))
                                {
                                    continue;
                                }
                                if (ISKTV.Equals("1") && ISKTT.Equals("0") && quyenGD >= 1 && !LANHDAOKYDUYET.Equals("1"))
                                {
                                    continue;
                                }
                                if (ISKTV.Equals("1") && ISKTT.Equals("0") && quyenLD >= 1 && LANHDAOKYDUYET.Equals("1"))
                                {
                                    continue;
                                }
                            }
                            dtR.Add(row);
                        }
                    }
                    foreach (DataRow row in dtR)
                    {
                        dt.Rows.Remove(row);
                    }
                    return dt.Rows.Count;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public static int GetDataBanHangDuyet(int npp_fk, int congty_fk, String userId)
        {
            try
            {
                using (var db = new clsDB())
                {

                    String query = $@"  SELECT count(1) cnt FROM (
                                        select a.PK_SEQ as ID, N'Đơn hàng bán' as loaichungtu,a.NgayDonHang as ngaychungtu, isnull(a.GHICHU,'') as noidung
                                         from ERP_Dondathang a  
                                         OUTER APPLY ( SELECT COUNT(1) AS SODONG FROM ERP_DONDATHANG_SANPHAM DHSP WHERE DHSP.DONDATHANG_FK = A.PK_SEQ) SODONG  	
                                         OUTER APPLY ( SELECT COUNT(1) AS SODONG FROM ERP_DONDATHANG_SANPHAM DHSP WHERE DHSP.DONDATHANG_FK = A.PK_SEQ AND DHSP.KHOTT_FK  in ( (select KHO_fK from NHANVIEN_KHO where nhanvien_fk = '{userId}') ) ) SODONGKHO  	
                                         OUTER APPLY ( SELECT COUNT(1) AS SODONG 
                                         FROM ERP_DONDATHANG DH  WHERE DH.PK_SEQ = A.PK_SEQ  and a.kho_fk in ( (select KHO_fK from NHANVIEN_KHO where nhanvien_fk = '{userId}') )  ) SODONGTONG 
                                         where a.pk_seq > 0 and a.trangthai in ('1') 
                                         and a.LoaiDonHang <> '6'  AND (CASE WHEN A.LOAIDONHANG IN (0,6) THEN (CASE WHEN  ISNULL(SODONG.SODONG,0) = ISNULL(SODONGKHO.SODONG,0)  THEN 1 ELSE 0 END) ELSE ISNULL(SODONGTONG.SODONG,0)  END) > 0   
                                         and a.npp_fk = '{npp_fk}' 
                                        ) as data
                                        ";
                    DataTable dt = db.getDataTable(query);
                    foreach (DataRow row in dt.Rows)
                    {
                        return int.Parse(row["cnt"].ToString());
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}