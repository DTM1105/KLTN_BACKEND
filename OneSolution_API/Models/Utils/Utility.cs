using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.Utils
{
    public class Utility
    {

        public static string GetDateTime()
        {
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("yyyy-MM-dd");
            return formattedDate;
        }
        public String check_Khoasothang_hople(clsDB db, String nppId, String thang, String nam)
        {
            try
            {
                String query = "select CONGTY_FK from NHAPHANPHOI WHERE PK_SEQ=" + nppId + "";
                //String congtyId = db.getFirstStringValueSqlCatchException(query);

                //CHECK THANG KHOA SO CO HOP LE HAY KHONG ( CHI DUOC CHOT SAU THANG KHOA SO + 1 )
                query = "select TOP 1 THANGKS, NAM from ERP_KHOASOTHANG WHERE  NPP_FK=" + nppId + " order by NAM desc, THANGKS desc  ";
                String thangKS = "12";
                String namKS = "2016";

                DataTable rsCheck = db.getDataTable(query);
                thangKS = rsCheck.Rows[0]["THANGKS"].ToString();
                namKS = rsCheck.Rows[0]["NAM"].ToString();

                rsCheck.Clone();

                String thangHopLe = "";
                String namHopLe = "";
                if (int.Parse(thangKS) == 12)
                {
                    thangHopLe = "1";
                    namHopLe = (int.Parse(namKS) + 1).ToString();
                }
                else
                {
                    thangHopLe = (int.Parse(thangKS) + 1).ToString();
                    namHopLe = namKS;
                }

                if ((int.Parse(namHopLe) > int.Parse(nam))
                        || (int.Parse(namHopLe) == int.Parse(nam) && int.Parse(thangHopLe) > int.Parse(thang)))
                {
                    return "Lỗi khóa sổ: Bạn chỉ có thể làm nghiệp vụ sau tháng khóa sổ gần nhất ( " + thangKS + "-" + namKS + " )";
                }

            }
            catch (Exception er)
            {

                return "Lỗi trong quá trình kiểm tra dữ liệu khóa sổ.Vui lòng thử lại.  ";

            }

            return "";
        }

        public String Update_KhoTT_NEW_old(String ngayyeucau, String ghichu, clsDB db, String khott_fk,
            String spId, String solo, String ngayhethan, String ngaynhapkho,
            String MAME, String MATHUNG, String vitri,
            String MAPHIEU, String phieudt, String phieueo,
            String MARQ, String HAMLUONG, String HAMAM,
            String loaidoituong, String doituongId,
            double soluong, double booked, double available, double dongia, String ngaysanxuat, String nsx_fk,string nppId)
        {
            string query = "";
            //THỐNG NHẤT LÀM TRÒN 3 CHỮ SỐ SAU SỐ LƯỢNG
            available = Math.Round(available, 4);
            soluong = Math.Round(soluong, 4);
            booked = Math.Round(booked, 4);

            if (HAMAM.Equals("0.0"))
                HAMAM = "0";
            if (HAMLUONG.Equals("100.0"))
                HAMLUONG = "100";

            if (HAMAM.Trim().Length <= 0)
                HAMAM = "0";
            if (HAMLUONG.Trim().Length <= 0)
                HAMLUONG = "100";

            if (vitri == null || vitri.Trim().Equals(""))
            {
                vitri = "0";
            }
            if (nsx_fk == null || nsx_fk.Equals("NULL") || nsx_fk.Trim().Equals(""))
            {
                nsx_fk = "0";
            }

            try
            {
                

                 query = "  select sanpham_fk,  cast( booked as numeric(18,4)) as booked ,cast( available as numeric(18,4))  as  available , cast(soluong as numeric(18,4)) as soluong " +
                              ", sp.ma + ' ' + sp.ten as ten   " +
                              "  from ERP_KHOTT_SP_CHITIET kho " +
                              "  inner join ERP_SANPHAM sp  on kho.sanpham_fk = sp.pk_seq " +
                              " where KHOTT_FK = " + khott_fk + " and kho.npp_fk=" + nppId + " and sanpham_fk = " + spId + " and solo = '" + solo + "' and ngayhethan = '" + ngayhethan + "' and ngaynhapkho = '" + ngaynhapkho + "' ";

                if (doituongId.Trim().Length > 0)
                {
                    query += " and kho.loaidoituong = '" + loaidoituong + "' and kho.doituongId = '" + doituongId + "' ";
                }
                else
                {
                    query += "  and kho.doituongId is null   ";
                }

                //if( MAME.Trim().Length > 0 )
                query += " and isnull(kho.MAME, '') = '" + MAME + "' ";
                //if( HAMLUONG.Trim().Length > 0 )
                query += " and isnull(kho.MATHUNG, '') = '" + MATHUNG + "' ";
                //	if( HAMAM.Trim().Length > 0 )
                query += " and isnull(kho.MAPHIEU, '') = '" + MAPHIEU + "' ";

                //	if( MARQ.Trim().Length > 0 )
                query += " and isnull(kho.MARQ, '') = N'" + MARQ + "' ";
                //	if( HAMLUONG.Trim().Length > 0 )
                query += " and isnull(kho.HAMLUONG, 100) = '" + HAMLUONG + "' ";
                //	if( HAMAM.Trim().Length > 0 )
                query += " and isnull(kho.HAMAM, 0) = '" + HAMAM + "' ";

                if (vitri.Trim().Length > 0)
                {
                    query += " and isnull(kho.bin_fk, 0 ) = " + vitri;
                }
                else
                {
                    query += " and isnull(kho.bin_fk, 0 ) = 0";
                }
                //if( phieudt.Trim().Length > 0 )
                query += " and isnull(kho.maphieudinhtinh, '') = '" + phieudt + "' ";
                //if( phieueo.Trim().Length > 0 )
                query += " and isnull(kho.phieueo, '') = '" + phieueo + "' ";

                if(ngaysanxuat.Trim().Length >0)
                    query +=" and isnull(kho.ngaysanxuat,'')= '"+ ngaysanxuat+"'";

                if (nsx_fk.Trim().Length > 0)
                {
                    query += " and isnull(kho.NSX_FK, 0 ) = " + nsx_fk;
                }
                else
                {
                    query += " and isnull(kho.NSX_FK, 0 ) = 0";
                }

                double available_ton = 0;
                //			double giaton = 0;
                double soluongton = 0;
                double booked_ton = 0;
                Boolean daco = false;

                DataTable dtCheck_t = db.getDataTable(query);

                if (dtCheck_t.Rows.Count > 0)
                {
                    for (int index_t = 0; index_t < dtCheck_t.Rows.Count; index_t++)
                    {
                        daco = true;
                        booked_ton = Double.Parse(dtCheck_t.Rows[index_t]["booked"].ToString());
                        soluongton = Math.Round(Double.Parse(dtCheck_t.Rows[index_t]["soluong"].ToString()), 4);
                        available_ton = Math.Round(Double.Parse(dtCheck_t.Rows[index_t]["available"].ToString()), 4);

                        if (available < 0 && available_ton < (-1) * available)
                        {
                            return "Số lượng tồn hiện tại trong kho " + khott_fk + " của sản phẩm : " + spId + " - " + dtCheck_t.Rows[index_t]["ten"].ToString() + " " +
                                    " [" + available_ton + "] / [" + available + "], số lô [" + solo + "], ngày hết hạn [" + ngayhethan + "],hàm ẩm: " + HAMAM + " ,hàm lượng : " + HAMLUONG + " " +
                                            "ngày nhập kho [" + ngaynhapkho + "]," +
                                                    " không đủ để trừ kho, vui lòng kiểm tra lại xuất nhập tồn của sản phẩm này ";
                        }

                        if (soluong < 0 && soluongton < (-1) * soluong)
                        {
                            //System.out.println(":: SO LUONG: " + soluong + " -- SO LUONG TON: " + soluongton);
                            return "Số lượng tồn trong kho của sản phẩm : " + dtCheck_t.Rows[index_t]["ten"].ToString() + "  [" + soluongton + "] không đủ để trừ kho, vui lòng kiểm tra lại xuất nhập tồn của sản phẩm này ";
                        }

                        String querylog = " INSERT INTO log_erp_khott_sp_chitiet ( npp_fk,KHOTT_FK	,SANPHAM_FK,	NGAYHETHAN,	SOLO	,BIN_FK	,SOLUONG	,BOOKED, " +
                                            "	AVAILABLE	,NGAYSANXUAT,	  NGAYNHAPKHO	, 	 	MARQ,	HAMLUONG,	HAMAM	,LOAIDOITUONG	,DOITUONGID	,MAME	,MATHUNG, " +
                                            "	MAPHIEU,	MAPHIEUDINHTINH	,PHIEUEO    ,DIENGIAI	,SOLUONG_CN,	BOOKED_CN,	AVAILABLE_CN , nsx_fk) " +
                                            "   SELECT "+nppId+"," + khott_fk + "	," + spId + ",	'" + ngayhethan + "',	'" + solo + "'	," + vitri + "	," + soluongton + "	," + booked_ton + ",	 " +
                                            "   " + available_ton + "	,'" + ngaysanxuat + "',	  '" + ngaynhapkho + "'	, N'" + MARQ + "',	'" + HAMLUONG + "',	'" + HAMAM + "'	,'" + loaidoituong + "'	,'" + doituongId + "'" +
                                            "	,'" + MAME + "'	,'" + MATHUNG + "',	'" + MAPHIEU + "',	'" + phieudt + "'	,'" + phieueo + "' ,N'" + ghichu + "'," + soluong + "," + booked + "," + available + "," + (nsx_fk.Trim().Length > 0 ? nsx_fk.Trim() : "NULL") + "";

                        if (db.updateQueryReturnInt(querylog) <= 0)
                        {
                            return " Không thể cập nhật log " + query;
                        }

                        query = " Update ERP_KHOTT_SP_CHITIET set booked = CAST( round(ISNULL(booked,0), 4) as   numeric(18,4) ) +  cast( round(" + booked + ", 4)  as   numeric(18,4) ), soluong =CAST( round(ISNULL(soluong,0), 4) as   numeric(18,4) ) +  cast( round(" + soluong + ", 4)  as   numeric(18,4) ), " +
                                " 	AVAILABLE = CAST( round(ISNULL(AVAILABLE,0), 4) as   numeric(18,4) ) +  cast( round(" + available + ", 4)  as   numeric(18,4) )   " +
                                "  where KHOTT_FK = " + khott_fk + " and npp_fk="+nppId+" and sanpham_fk = " + spId + " and solo = '" + solo + "' and ngayhethan = '" + ngayhethan
                                + "' and ngaynhapkho = '" + ngaynhapkho + "' ";

                        if (doituongId.Trim().Length > 0)
                        {
                            query += " and loaidoituong = '" + loaidoituong + "' and doituongId = '" + doituongId + "' ";
                        }
                        else
                        {
                            query += "  and doituongId is null  ";
                        }

                        //if( MAME.Trim().Length > 0 )
                        query += " and isnull(MAME, '') = '" + MAME + "' ";
                        //	if( HAMLUONG.Trim().Length > 0 )
                        query += " and isnull(MATHUNG, '') = '" + MATHUNG + "' ";
                        //if( HAMAM.Trim().Length > 0 )
                        query += " and isnull(MAPHIEU, '') = '" + MAPHIEU + "' ";

                        //if( MARQ.Trim().Length > 0 )
                        query += " and isnull(MARQ, '') = N'" + MARQ + "' ";

                        if (HAMLUONG.Trim().Length > 0)
                            query += " and isnull(HAMLUONG, 100) = '" + HAMLUONG + "' ";
                        if (HAMAM.Trim().Length > 0)
                            query += " and isnull(HAMAM, 0) = '" + HAMAM + "' ";

                        if (vitri.Trim().Length > 0)
                        {
                            query += " and isnull(bin_fk, 0 ) = " + vitri;
                        }
                        else
                        {
                            query += " and isnull(bin_fk, 0 ) = 0 ";
                        }

                        //if( phieudt.Trim().Length > 0 )
                        query += " and isnull(maphieudinhtinh, '') = '" + phieudt + "' ";
                        //if( phieueo.Trim().Length > 0 )
                        query += " and isnull(phieueo, '') = '" + phieueo + "' ";

                        if(ngaysanxuat.Trim().Length >0)
                            query +=" and isnull(ngaysanxuat,'')= '"+ ngaysanxuat+"'";

                        if (nsx_fk.Trim().Length > 0)
                        {
                            query += " and isnull(nsx_fk, 0 ) = " + nsx_fk;
                        }
                        else
                        {
                            query += " and isnull(nsx_fk, 0 ) = 0";
                        }

                        //System.out.println("::: 2. CAP NHAT KHO CT: " + query);
                        int resultInt = db.updateQueryReturnInt(query);
                        if (resultInt != 1)
                        {
                            return " Không thể cập nhật ERP_KHOTT_SP_CHITIET " + query;
                        }
                    }
                }

                dtCheck_t.Clone();

                if (!daco)  //Trường hợp trong kho chi tiết chưa có SP NÀY
                {
                    String querylog = " INSERT INTO log_erp_khott_sp_chitiet (npp_fk, KHOTT_FK	,SANPHAM_FK,	NGAYHETHAN,	SOLO	,BIN_FK	,SOLUONG	,BOOKED, " +
                    "	AVAILABLE	,NGAYSANXUAT,	  NGAYNHAPKHO	, 	 	MARQ,	HAMLUONG,	HAMAM	,LOAIDOITUONG	,DOITUONGID	,MAME	,MATHUNG, " +
                    "	MAPHIEU,	MAPHIEUDINHTINH	,PHIEUEO    ,DIENGIAI	,SOLUONG_CN,	BOOKED_CN,	AVAILABLE_CN , nsx_fk) " +
                    "   SELECT "+nppId+"," + khott_fk + "	," + spId + ",	'" + ngayhethan + "',	'" + solo + "'	," + vitri + "	," + soluongton + "	," + booked_ton + ",	 " +
                    "   " + available_ton + "	,'" + ngaysanxuat + "',	  '" + ngaynhapkho + "'	, N'" + MARQ + "',	'" + HAMLUONG + "',	'" + HAMAM + "'	,'" + loaidoituong + "'	,'" + doituongId + "'" +
                    "	,'" + MAME + "'	,'" + MATHUNG + "',	'" + MAPHIEU + "',	'" + phieudt + "'	,'" + phieueo + "' ,N'" + ghichu + "'," + soluong + "," + booked + "," + available + "," + (nsx_fk.Trim().Length > 0 ? nsx_fk.Trim() : "NULL") + "";

                    if (db.updateQueryReturnInt(querylog) <= 0)
                    {
                        return " Không thể cập nhật log " + query;
                    }

                    query = "insert ERP_KHOTT_SP_CHITIET( npp_fk,KHOTT_FK, SANPHAM_FK, SOLO, NGAYHETHAN, NGAYNHAPKHO, soluong, booked, available ";
                    if (doituongId.Trim().Length > 0)
                        query += " ,loaidoituong, doituongId ";

                    if (MAME.Trim().Length > 0)
                        query += " ,MAME ";
                    if (MATHUNG.Trim().Length > 0)
                        query += " ,MATHUNG ";
                    if (MAPHIEU.Trim().Length > 0)
                        query += " ,MAPHIEU ";

                    if (MARQ.Trim().Length > 0)
                        query += " ,MARQ ";
                    if (HAMLUONG.Trim().Length > 0)
                        query += " ,HAMLUONG ";
                    if (HAMAM.Trim().Length > 0)
                        query += " ,HAMAM ";

                    if (vitri.Trim().Length > 0)
                        query += " ,BIN_FK, KHUVUCKHO_FK ";
                    if (phieudt.Trim().Length > 0)
                        query += " ,maphieudinhtinh ";
                    if (phieueo.Trim().Length > 0)
                        query += " ,phieueo ";
                    if (ngaysanxuat.Trim().Length > 0)
                        query += " ,ngaysanxuat ";

                    if (nsx_fk.Trim().Length > 0)
                        query += " ,NSX_FK ";

                    query += " )";

                    query += " select "+nppId+",'" + khott_fk + "', " + spId + ", '" + solo + "', '" + ngayhethan + "', '" + ngaynhapkho
                            + "', " + soluong + ", " + booked + ", " + available + "";
                    if (doituongId.Trim().Length > 0)
                        query += " ,'" + loaidoituong + "', '" + doituongId + "' ";

                    if (MAME.Trim().Length > 0)
                        query += " ,'" + MAME + "' ";
                    if (MATHUNG.Trim().Length > 0)
                        query += " , '" + MATHUNG + "' ";
                    if (MAPHIEU.Trim().Length > 0)
                        query += " , '" + MAPHIEU + "' ";

                    if (MARQ.Trim().Length > 0)
                        query += " , N'" + MARQ + "' ";
                    if (HAMLUONG.Trim().Length > 0)
                        query += " , '" + HAMLUONG + "' ";
                    if (HAMAM.Trim().Length > 0)
                        query += " , '" + HAMAM + "' ";

                    if (vitri.Trim().Length > 0)
                        query += " , " + vitri + ", ( select KHUVUC_FK from ERP_BIN where PK_SEQ = " + vitri + " ) ";
                    if (phieudt.Trim().Length > 0)
                        query += " , '" + phieudt + "' ";
                    if (phieueo.Trim().Length > 0)
                        query += " , '" + phieueo + "' ";
                    if (ngaysanxuat.Trim().Length > 0)
                        query += " , '" + ngaysanxuat + "' ";

                    if (nsx_fk.Trim().Length > 0)
                        query += " , " + nsx_fk + " ";

                    query += "  ";
                    //System.out.println("::: INSERT KHO CT: " + query);
                    int resultInt = db.updateQueryReturnInt(query);
                    if (resultInt != 1)
                    {
                        return " Không thể thêm mới ERP_KHOTT_SP_CHITIET " + query;
                    }
                }

                if (!daco)
                {
                    if (soluong < 0 || available < 0 || booked < 0)
                    {
                        return "Số lượng tồn trong kho không hợp lệ. Vui lòng liên hệ Admin để xử lý ";
                    }
                }

                // kiểm tra sp lô,ngày nhập kho, có bị âm  kho ko?
                /*if (soluong < 0)
                {
                    query = " SELECT * FROM [UFN_NXT_HO_FULL_THEO_SP_NSX]('','" + ngayyeucau + "'," + khott_fk + "," + spId + "," + vitri + ",'" + solo + "','" + ngaynhapkho + "','" + ngayhethan + "','" + MAME + "','" + MATHUNG + "','" + MAPHIEU + "','" + phieudt + "','" + phieueo + "','" + MARQ + "','" + HAMAM + "','" + HAMLUONG + "'," + (nsx_fk.Trim().Length == 0 ? "0" : nsx_fk) + ")  " +
                            " WHERE ROUND(CUOIKY,3) <0";

                    DataTable dt = db.getDataTable(query);
                    for (int index_t = 0; index_t < dt.Rows.Count; index_t++)
                    {
                        return "Vui lòng thử lại nghiệp vụ,nếu không được vui lòng báo Admin để được trợ giúp. Tổng xuất nhập tồn của sản phẩm :" + spId + " | số lô :" + solo + " bạn đang cập nhật không hợp lệ";
                    }
                    dt.Clone();
                }*/
            }
            catch (Exception er)
            {
                return "không thể thực hiện cập nhật kho  Util.Update_KhoTT : " + er.Message+ " --- " +query;
            }

            return "";
        }




        public String Update_KhoTT_NSX(String ngayyeucau, String ghichu, clsDB db, String npp_fk, String khott_fk,
                String spId, String solo, String ngayhethan, String ngaynhapkho,
                String vitri, String MAPHIEU, String tinhtrang,
                String loaidoituong, String doituongId,
                double soluong, double booked, double available, String ngaysanxuat, String Chitietkho_fk,string barcode_fk123)
        {

            if (Chitietkho_fk == null)
            {
                Chitietkho_fk = "";
            }
            //THỐNG NHẤT LÀM TRÒN 3 CHỮ SỐ SAU SỐ LƯỢNG
            int lamtronsoluong = 3;
            //System.out.println("lamtronsoluong:" +lamtronsoluong);
            available = Math.Round(available, lamtronsoluong);
            soluong = Math.Round(soluong, lamtronsoluong);
            booked = Math.Round(booked, lamtronsoluong);

            if (loaidoituong == null || loaidoituong.Equals("null") || loaidoituong.Equals("NULL"))
            {
                loaidoituong = "";
            }
            if (doituongId == null)
            {
                doituongId = "";
            }
            if (ngaysanxuat == null || ngaysanxuat.Trim().Length == 0) ngaysanxuat = "";

            try
            {

                if (ngayyeucau == null || ngayyeucau.Equals(""))
                {
                    return "Không xác định được ngày nghi nhận nghiệp vụ ";
                }
                if (ngaysanxuat == null || ngaysanxuat.Equals(""))
                {
                    return "Vui lòng nhập ngày sản xuất ";
                }
                if (ngayhethan == null || ngayhethan.Equals(""))
                {
                    return "Vui lòng nhập ngày hết hạn ";
                }
                if (ngayhethan.CompareTo(ngaysanxuat) < 0)
                    return "Vui lòng nhập ngày hết hạn lớn hơn ngày sản xuất";

                String thang = ngayyeucau.Substring(5, 2);
                String nam = ngayyeucau.Substring(0, 4);
                String msg1 = this.check_Khoasothang_hople(db, npp_fk, thang,nam);

                if (msg1.Trim().Length > 0)
                {
                    return "Lỗi nghiệp vụ :" + msg1;
                }

                String query = "  select sanpham_fk, available, booked,soluong, sp.ma + ' ' + sp.ten as ten   " +
                        "  from ERP_KHOTT_SP_CHITIET kho " +
                        "  inner join ERP_SANPHAM sp  on kho.sanpham_fk = sp.pk_seq " +
                        " where kho.NPP_FK = '" + npp_fk + "' and KHOTT_FK = " + khott_fk + " and sanpham_fk = " + spId + " " +
                        " and solo = '" + solo + "' and ngayhethan = '" + ngayhethan + "' and ngaynhapkho = '" + ngaynhapkho +
                        "' and ISNULL(ngaysanxuat,'')='" + ngaysanxuat + "'  ";

                if (doituongId.Trim().Length > 0)
                {
                    query += " and kho.loaidoituong = '" + loaidoituong + "' and kho.doituongId = '" + doituongId + "' ";
                }
                else
                {
                    query += "  and kho.doituongId is null   ";
                }

                if (tinhtrang == null || tinhtrang.Trim().Length <= 0)
                    tinhtrang = "-1";

                query += " and isnull(kho.tinhtrang, -1) = '" + tinhtrang + "' ";
                query += " and isnull(kho.MAPHIEU, '') = '" + MAPHIEU + "' ";

                if (vitri.Trim().Length > 0)
                {
                    query += " and isnull(kho.bin_fk, 0 ) = " + vitri;
                }
                else
                {
                    query += " and isnull(kho.bin_fk, 0 ) = 0";
                }

                if (this.ParseTringToDouble( Chitietkho_fk ) > 0 )
                {
                    query += " and isnull(kho.chitietkho_fk, 0 ) = " + Chitietkho_fk;
                }
                else
                {
                    query += " and isnull(kho.chitietkho_fk, 0 ) = 0 ";
                }

               
                //System.out.println("[UTILITY KHO : QUERY CHECK KHO]" + query);
                double available_ton = 0;
                //double giaton = 0;
                double soluongton = 0;
                double booked_ton = 0;
                bool daco = false;

                DataTable dt = db.getDataTable(query);
                String querylog = "";
                if (dt.Rows.Count>0)
                {
                    foreach(DataRow rsCheck in dt.Rows)
                    {
                        daco = true;
                        soluongton = Math.Round(XuLy.parseDouble( rsCheck["soluong"].ToString()), lamtronsoluong);
                        available_ton = Math.Round(XuLy.parseDouble(rsCheck["available"].ToString()), lamtronsoluong);
                        booked_ton = XuLy.parseDouble(rsCheck["booked"].ToString());

                        //System.out.println("::: SO LUONG: " + soluongton + "  -- BOOKED: " + booked_ton + " -- AVAI TON: " + available_ton );


                        if (available < 0 && available_ton < (-1) * available)
                        {
                            return "Số lượng tồn hiện tại trong kho của sản phẩm : " + rsCheck["ten"] + "  [" + available_ton + "], số lô [" + solo + "], ngày hết hạn [" + ngayhethan + "], ngày nhập kho [" + ngaynhapkho + "], không đủ để trừ kho, vui lòng kiểm tra lại xuất nhập tồn của sản phẩm này ";
                        }

                        if (soluong < 0 && soluongton < (-1) * soluong)
                        {
                            //System.out.println(":: SO LUONG: " + soluong + " -- SO LUONG TON: " + soluongton);
                            return "Số lượng tồn trong kho của sản phẩm : " + rsCheck["ten"] + "  [" + soluongton + "] không đủ để trừ kho, vui lòng kiểm tra lại xuất nhập tồn của sản phẩm này ";
                        }

                         querylog = " INSERT INTO log_erp_khott_sp_chitiet ( NPP_FK, KHOTT_FK, SANPHAM_FK,	NGAYHETHAN,	SOLO	,BIN_FK	,SOLUONG	,BOOKED, " +
                                "	AVAILABLE, NGAYSANXUAT, NGAYNHAPKHO,LOAIDOITUONG	,DOITUONGID	," +
                                "	MAPHIEU, TINHTRANG, DIENGIAI, SOLUONG_CN,	BOOKED_CN,	AVAILABLE_CN ,CHITIETKHO_FK) " +
                                "   SELECT '" + npp_fk + "', " + khott_fk + "	," + spId + ", '" + ngayhethan + "',	'" + solo + "'	," + (vitri != null && vitri.Trim().Length > 0 ? vitri : "null") + "	," + soluongton + "	," + booked_ton + ",	 " +
                                "   " + available_ton + "	,'" + ngaysanxuat + "', '" + ngaynhapkho + "'	, '" + loaidoituong + "'	," + (doituongId.Trim().Length > 0 ? doituongId : "null") + "," +
                                "	'" + MAPHIEU + "', '" + tinhtrang + "', N'" + ghichu + "'," + soluong + "," + booked + "," + available + "," + (Chitietkho_fk.Length > 0 ? Chitietkho_fk : "NULL");

                        int resultInt = db.updateQueryReturnInt(querylog);
                        if (resultInt != 1)
                        {
                            //System.out.println("::: SO DONG: " + resultInt);
                            return " --- Không thể cập nhật Log  ( " + resultInt + " ) " + querylog;
                        }
                        query = " Update ERP_KHOTT_SP_CHITIET set   booked = round(isnull(booked,0), " + lamtronsoluong + ") + round(" + booked + ", " + lamtronsoluong + "), soluong = round(ISNULL(soluong,0), " + lamtronsoluong + ") + round(" + soluong + ", " + lamtronsoluong + "), " +
                                " 	AVAILABLE = round(ISNULL(AVAILABLE,0), " + lamtronsoluong + ") + round(" + available + ", " + lamtronsoluong + ")  " +
                                "  where NPP_FK = '" + npp_fk + "' and KHOTT_FK = " + khott_fk + " and sanpham_fk = " + spId + " and solo = '" + solo + "' and ngayhethan = '" + ngayhethan + "' and ngaynhapkho = '" + ngaynhapkho + "' and ISNULL(ngaysanxuat,'')='" + ngaysanxuat + "' ";

                        if (doituongId.Trim().Length > 0)
                        {
                            query += " and  loaidoituong = '" + loaidoituong + "' and  doituongId = '" + doituongId + "' ";
                        }
                        else
                        {
                            query += "  and  doituongId is null   ";
                        }

                        query += " and isnull( tinhtrang, -1) = '" + tinhtrang + "' ";
                        query += " and isnull( MAPHIEU, '') = '" + MAPHIEU + "' ";

                        if (vitri.Trim().Length > 0)
                        {
                            query += " and isnull( bin_fk, 0 ) = " + vitri;
                        }
                        else
                        {
                            query += " and isnull( bin_fk, 0 ) = 0";
                        }
                        if (ParseTringToDouble (Chitietkho_fk) > 0)
                        {
                            query += " and isnull( chitietkho_fk, 0 ) = " + Chitietkho_fk;
                        }
                        else
                        {
                            query += " and isnull( chitietkho_fk, 0 ) = 0 ";
                        }
                        

                        ////System.out.println("::: 1.CAP NHAT KHO CT: " + query);
                        resultInt = db.updateQueryReturnInt(query);
                        if (resultInt != 1)
                        {
                            //System.out.println("::: SO DONG: " + resultInt);
                            return " --- Không thể cập nhật ERP_KHOTT_SP_CHITIET  ( " + resultInt + " ) " + query;
                        }
                    }    
                    

                }


                if (!daco)  //Trường hợp trong kho chi tiết chưa có SP NÀY
                {

                     querylog = " INSERT INTO log_erp_khott_sp_chitiet ( NPP_FK, KHOTT_FK	,SANPHAM_FK,	NGAYHETHAN,	SOLO	,BIN_FK	,SOLUONG	,BOOKED, " +
                            "	AVAILABLE	,NGAYSANXUAT,	  NGAYNHAPKHO,  LOAIDOITUONG, DOITUONGID, " +
                            "	MAPHIEU,  TINHTRANG, DIENGIAI	,SOLUONG_CN,	BOOKED_CN,	AVAILABLE_CN ,CHITIETKHO_FK ) " +
                            "   SELECT '" + npp_fk + "', " + khott_fk + "	," + spId + ",	'" + ngayhethan + "',	'" + solo + "'	," + (vitri != null && vitri.Trim().Length > 0 ? vitri : "null") + "	," + soluongton + "	," + booked_ton + ",	 " +
                            "   " + available_ton + "	,'" + ngaysanxuat + "',	  '" + ngaynhapkho + "'	,'" + loaidoituong + "'	," + (doituongId.Trim().Length > 0 ? doituongId : "null") + ", " +
                            "	N'" + MAPHIEU + "', '" + tinhtrang + "', N'" + ghichu + "'," + soluong + "," + booked + "," + available + "," + (Chitietkho_fk.Length > 0 ? Chitietkho_fk : "NULL");
                    //System.out.println("::: INSERT KHO log: " + querylog);
                    int resultInt = db.updateQueryReturnInt(querylog);
                    if (resultInt != 1)
                    {
                        //System.out.println("::: SO DONG: " + resultInt);
                        return " Không thể thêm mới Log " + query;
                    }


                    query = "insert ERP_KHOTT_SP_CHITIET( NPP_FK, KHOTT_FK, SANPHAM_FK, SOLO, NGAYHETHAN, NGAYNHAPKHO,ngaysanxuat, "
                            + "\n	soluong, booked, available ";
                    if (doituongId.Trim().Length  > 0)
                        query += " ,loaidoituong, doituongId ";

                    if (MAPHIEU.Trim().Length > 0)
                        query += " ,MAPHIEU ";

                    if (tinhtrang.Trim().Length > 0)
                        query += " ,tinhtrang ";

                    if (vitri.Trim().Length > 0)
                        query += " ,BIN_FK ";

                    query += ", CHITIETKHO_FK  )";

                    query += " select  '" + npp_fk + "', '" + khott_fk + "', " + spId + ", '" + solo + "', "
                            + "\n	'" + ngayhethan + "', '" + ngaynhapkho + "','" + ngaysanxuat + "', " + soluong + ", "
                            + "\n	" + booked + ", " + available + " ";
                    if (doituongId.Trim().Length > 0)
                        query += " ,'" + loaidoituong + "', '" + doituongId + "' ";

                    if (MAPHIEU.Trim().Length > 0)
                        query += " , '" + MAPHIEU + "' ";

                    if (tinhtrang.Trim().Length > 0)
                        query += " , '" + tinhtrang + "' ";

                    if (vitri.Trim().Length > 0)
                        query += " , " + vitri + "  ";

                    query += " ," + (Chitietkho_fk.Length > 0 ? Chitietkho_fk : "NULL") + "";
                     

                  query += "  ";

                    //System.out.println("::: INSERT KHO CT: " + query);
                    resultInt = db.updateQueryReturnInt(query);
                    if (resultInt != 1)
                    {
                        //System.out.println("::: SO DONG: " + resultInt);
                        return " Không thể thêm mới ERP_KHOTT_SP_CHITIET " + query;
                    }
                }

                if (!daco)
                {
                    if (soluong < 0 || available < 0 || booked < 0)
                    {
                        return "Số lượng tồn trong kho không hợp lệ. Vui lòng liên hệ Admin để xử lý ";
                    }
                }

            

            }
            catch (Exception er)
            {
                
                return "không thể thực hiện cập nhật kho  Util.Update_KhoTT : " + er.Message;
            }

            return "";
        }

        private double ParseTringToDouble(string str)
        {
            try
            {
                return double.Parse(str.Replace(",", ""));
            }catch(Exception er)
            {
                return 0;
            }
        }

        private double GetGia_ChayKT_Dauky(String spid, clsDB db,
            Boolean ischaycuoiky, String thangtruoc, String namtruoc, String ngaychungtu)
        {
            double dongia = 0;

            try
            {
                if (!ischaycuoiky)
                {
                    String query = " SELECT SANPHAM_FK, round(ISNULL(DONGIA, 0), 0) GIATON  from ERP_BANGGIA_THANHPHAM_CUOIKY    " +
                                    " WHERE SANPHAM_FK = " + spid + " AND  THANG = '" + thangtruoc + "' " +
                                    " AND NAM = '" + namtruoc + "'  ";

                    DataTable dtgia = db.getDataTable(query);
                    int bien = 0;
                    for (int index = 0; index < dtgia.Rows.Count; index++)
                    {
                        dongia = Double.Parse(dtgia.Rows[index]["GIATON"].ToString());
                        bien++;
                    }
                    dtgia.Clone();

                    if (bien == 0)
                    {
                        String query3 = "SELECT TOP 1 DONGIA * isnull(MH.TyGiaQuyDoi,1) AS DONGIA , NH.NGAYNHAN " +
                                        "FROM ERP_NHANHANG NH " +
                                        "LEFT JOIN  ERP_NHANHANG_SANPHAM NHSP ON NH.PK_SEQ=NHSP.NHANHANG_FK "
                                        + " INNER JOIN ERP_MUAHANG MH ON MH.PK_SEQ= NH.MUAHANG_FK  " +
                                        "WHERE     nh.trangthai=1  and NHSP.SANPHAM_FK =" + spid + " and NH.NGAYNHAN <= '" + ngaychungtu + "'  " +
                                        "ORDER BY NH.NGAYNHAN DESC ";

                        DataTable dt3 = db.getDataTable(query3);
                        for (int index = 0; index < dt3.Rows.Count; index++)
                        {
                            dongia = Double.Parse(dt3.Rows[index]["DONGIA"].ToString());
                        }
                        dt3.Clone();
                    }
                }
                else
                {
                    String query = " SELECT SANPHAM_FK,  DONGIA_2  from ERP_BANGGIA_TON_CUOIKY " +
                    " WHERE SANPHAM_FK = " + spid + " AND  THANG = '" + thangtruoc + "' " +
                    " AND NAM = '" + namtruoc + "'  ";

                    DataTable dtgia = db.getDataTable(query);
                    for (int index = 0; index < dtgia.Rows.Count; index++)
                    {
                        dongia = Double.Parse(dtgia.Rows[index]["DONGIA_2"].ToString());
                    }
                    dtgia.Clone();
                }
            }
            catch (Exception er)
            {
                //er.Message;
            }
            return dongia;
        }

        public String Revert_KeToan_loaichungtu(clsDB db, String loaichungtu, String sochungtu)
        {
            try
            {
                //GHI NHAN NGUOC LAI TAI KHOAN NO - CO
                String query = " delete  ERP_PHATSINHKETOAN " +
                                " where LOAICHUNGTU = N'" + loaichungtu.Trim() + "' and SOCHUNGTU = '" + sochungtu + "'  ";

                if (!db.updateQuery(query))
                {
                    return "Không thể hủy ERP_PHATSINHKETOAN " + query;
                }
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public String[] getThangNam(clsDB  db, Boolean Ischaycuoiky, String ngaychotnv)
        {
            String[] mang = new String[3];
            try
            {
                if (Ischaycuoiky)
                {
                    // là tháng khóa sổ để lấy giá trong tháng
                    mang[0] = ngaychotnv.Substring(6, 2);
                    mang[1] = ngaychotnv.Substring(0, 4);
                    mang[2] = "";
                }
                else
                {

                    String query = "select top 1 THANGKS,NAM from ERP_KHOASOTHANG order by NAM desc,THANGKS desc";
                    DataTable rsks = db.getDataTable(query);
                    if (rsks.Rows.Count > 0)
                    {
                        mang[0] = rsks.Rows[0]["THANGKS"].ToString();
                        mang[1] = rsks.Rows[0]["NAM"].ToString();
                        mang[2] = "";
                    }
                    else
                    {
                        mang[2] = "Không xác định được tháng khóa sổ gần nhất";
                    }
                    rsks.Clone();
                }
            }
            catch (Exception er)
            {
                mang[2] = er.Message;
            }
            return mang;

        }

        private String[] getThangNam_NPP(clsDB db, Boolean Ischaycuoiky, String ngaychotnv, String congty_fk)
        {
            String[] mang = new String[3];
            try
            {

                if (Ischaycuoiky)
                {
                    // là tháng khóa sổ để lấy giá trong tháng

                    mang[0] = ngaychotnv.Substring(6, 2);
                    mang[1] = ngaychotnv.Substring(0, 4);
                    mang[2] = "";
                }
                else
                {

                    String query = "select top 1 THANGKS,NAM from ERP_KHOASOKETOAN WHERE CONGTY_FK=" + congty_fk + " order by NAM desc,THANGKS desc";
                    DataTable rsks = db.getDataTable(query);
                    if (rsks.Rows.Count > 0)
                    {
                        mang[0] = rsks.Rows[0]["THANGKS"].ToString();
                        mang[1] = rsks.Rows[0]["NAM"].ToString();
                        mang[2] = "";
                    }
                    else
                    {
                        mang[2] = "Không xác định được tháng khóa sổ gần nhất";
                    }
                    rsks.Clone();

                }

            }
            catch (Exception er)
            {
                mang[2] = er.Message;

            }
            return mang;
        }

        public static double GetGia_ChayKT_NPP(String spid, clsDB db, Boolean ischaycuoiky, String thangtruoc, String namtruoc, String congty_fk, String ngayNghiepVu)
        {
            double dongia = 0;
            try
            {

                String query = " SELECT  DONGIA from ERP_BANGGIA_THANHPHAM_CUOIKY " +
                                " WHERE SANPHAM_FK = " + spid + "   " +
                                "  AND congty_fk = '" + congty_fk + "' and DONGIA > 0  "
                                 + "  ORDER BY dateadd(mm, (NAM - 1900) * 12 + THANG - 1 , 1 - 1)  DESC  ";





                dongia = db.getFirstDoubleValueSqlCatchException(query);


                if (dongia == null || dongia <= 0)
                {
                    query = "SELECT TOP 1 DONGIAVIET dongia " +
                                    "FROM ERP_NHANHANG NH " +
                                    "LEFT JOIN  ERP_NHANHANG_SANPHAM NHSP ON NH.PK_SEQ=NHSP.NHANHANG_FK " +
                                    "WHERE NHSP.SANPHAM_FK =" + spid + " and NH.NGAYNHAN <= '" + ngayNghiepVu + "'  " +
                                    "ORDER BY NH.NGAYNHAN DESC ";

                    dongia = db.getFirstDoubleValueSqlCatchException(query);



                }

            }
            catch (Exception er)
            {

            }

            return dongia;
        }

        public int chuyenInt(String intString)
        {
            int i = 0;
            if (!Int32.TryParse(intString, out i))
            {
                i = -1;
            }
            return i;
        }

        public String Update_TaiKhoan_SP_ERP(clsDB db, String thang, String nam, String ngaychungtu, String ngayghinhan, String loaichungtu, String sochungtu, String taikhoanNO_fk, String taikhoanCO_fk, String NOIDUNGNHAPXUAT_FK, String NO, String CO,
            String DOITUONG_NO, String MADOITUONG_NO, String DOITUONG_CO, String MADOITUONG_CO, String LOAIDOITUONG,
            String SOLUONG, String DONGIA, String TIENTEGOC_FK, String DONGIANT, String TIGIA_FK, String TONGGIATRI,
            String TONGGIATRINT, String khoanmuc, String SpId, String masp, String tensp, String donvi, String Solo, String KhoNhanId, String KhoXuatId, String Sohoadon, String Ngayhoadon, String KhoanmucchiphiId,String Nppid,String CtyId)
        {
            String msg = Check_NgayNghiepVu_KeToan(db, thang, nam);
            if (msg.Trim().Length > 0)
            {
                msg = "1.0 Không thể cập nhật tài khoản kế toán " + msg;
                return msg;
            }


            msg = "";
            if (taikhoanCO_fk == null || taikhoanCO_fk.Length == 0)
            {
                return "Chưa xác định được tài khoản có để cập nhật bút toán";
            }
            if (taikhoanNO_fk == null || taikhoanNO_fk.Length == 0)
            {
                return "Chưa xác định được tài khoản nợ để cập nhật bút toán";
            }

            if (ngayghinhan == null || ngayghinhan.Length == 0)
            {
                return "Chưa xác định được ngày ghi nhận bút toán ";
            }
            if (sochungtu == null || sochungtu.Length == 0)
            {
                return "Chưa xác định được số chứng từ ";
            }
            if (KhoNhanId == null || KhoNhanId.Length == 0)
            {
                KhoNhanId = "NULL";
            }
            if (KhoXuatId == null || KhoXuatId.Length == 0)
            {
                KhoXuatId = "NULL";
            }


            String _ndnhapxuat_fk = "null";
            if (NOIDUNGNHAPXUAT_FK.Trim().Length > 0)
                _ndnhapxuat_fk = NOIDUNGNHAPXUAT_FK;

            String _sochungtu = "null";
            if (sochungtu.Trim().Length > 0)
                _sochungtu = sochungtu;

            String _soluong = "null";
            if (SOLUONG.Trim().Length > 0)
                _soluong = SOLUONG.Trim();

            String _dongia = "null";
            if (DONGIA.Trim().Length > 0)
                _dongia = DONGIA.Trim();

            String _thanhtienViet = "null";
            if (TONGGIATRI.Trim().Length > 0)
                _thanhtienViet = TONGGIATRI.Trim();

            String _dongiaNT = "null";
            if (DONGIANT.Trim().Length > 0)
                _dongiaNT = DONGIANT.Trim();

            String _thanhtienNT = "null";
            if (TONGGIATRINT.Trim().Length > 0)
                _thanhtienNT = TONGGIATRINT.Trim();

            String _NO = "0";
            if (NO.Trim().Length > 0)
                _NO = NO;

            String _CO = "0";
            if (CO.Trim().Length > 0)
                _CO = CO;

            if (SpId != null && SpId.Equals(""))
            {
                SpId = "NULL";
            }

            if (KhoanmucchiphiId != null && KhoanmucchiphiId.Equals(""))
            {
                KhoanmucchiphiId = "NULL";
            }

            //GHI CO

            //GHI PHAT SINH VA DOI UNG CHO TAO KHOAN CO


            String query = "insert ERP_PHATSINHKETOAN ( NPP_FK,CONGTY_FK, ngaychungtu, ngayghinhan, loaichungtu, sochungtu, taikhoan_fk, taikhoandoiung_fk, NOIDUNGNHAPXUAT_FK, NO, CO, " +
                                                         "  DOITUONG,  MADOITUONG, LOAIDOITUONG, SOLUONG, DONGIA, TIENTEGOC_FK, DONGIANT, TIGIA_FK, TONGGIATRI, TONGGIATRINT, " +
                                                         " KHOANMUC,SOHOADON,NGAYHOADON,SANPHAM_FK,KHOANMUCCHIPHI_FK,KHO_FK,KHONHAN_FK ,MAHANG,TENHANG,DONVI,SOLO,IS_CO ) " +
                        "values ( "+Nppid+","+CtyId+",'" + ngaychungtu + "', '" + ngayghinhan + "', N'" + loaichungtu + "', " + _sochungtu + ", '" + taikhoanCO_fk + "', '" + taikhoanNO_fk + "', " + _ndnhapxuat_fk + ", '0', " + _CO + ", " +
                                        " N'" + DOITUONG_CO + "', N'" + MADOITUONG_CO + "', '" + LOAIDOITUONG + "', " + _soluong + ", " + _dongia + ", '" + TIENTEGOC_FK + "', " + _dongiaNT + ", '" + TIGIA_FK + "', " + _thanhtienViet + ", " +
                                                " " + _thanhtienNT + ", N'" + khoanmuc + "','" + Sohoadon + "','" + Ngayhoadon + "'," + SpId + ", " + KhoanmucchiphiId + " ," + KhoXuatId + "," + KhoNhanId + ",'" + masp + "',N'" + tensp + "',N'" + donvi + "','" + Solo + "','1' ) ";

            if (db.updateQueryReturnInt(query) <= 0)
            {
                msg = "3.Không thể cập nhật tài khoản kế toán " + query;
                return msg;
            }


            //GHI NO


            //GHI PHAT SINH VA DOI UNG CHO TAO KHOAN CO
            query = " insert ERP_PHATSINHKETOAN ( NPP_FK,CONGTY_FK, ngaychungtu, ngayghinhan, loaichungtu, sochungtu, taikhoan_fk, taikhoandoiung_fk, NOIDUNGNHAPXUAT_FK, NO, CO, " +
                    " DOITUONG,  MADOITUONG, LOAIDOITUONG, SOLUONG, DONGIA, TIENTEGOC_FK, DONGIANT, TIGIA_FK, TONGGIATRI, TONGGIATRINT, KHOANMUC , SOHOADON,NGAYHOADON,SANPHAM_FK,KHOANMUCCHIPHI_FK,KHO_FK,KHONHAN_FK ,MAHANG,TENHANG,DONVI,SOLO,IS_NO) " +
                    " values ( " + Nppid + "," + CtyId + ",'" + ngaychungtu + "', '" + ngayghinhan + "', N'" + loaichungtu + "', " + _sochungtu + ", '" + taikhoanNO_fk + "', '" + taikhoanCO_fk + "', " + _ndnhapxuat_fk + ", " + _NO + ", '0', " +
                    " N'" + DOITUONG_NO + "', N'" + MADOITUONG_NO + "', '" + LOAIDOITUONG + "', " + _soluong + ", " + _dongia + ", '" + TIENTEGOC_FK + "', " + _dongiaNT + ", '" + TIGIA_FK + "', " + _thanhtienViet + ", " + _thanhtienNT + ", N'" + khoanmuc + "' " +
                            " ,'" + Sohoadon + "','" + Ngayhoadon + "'," + SpId + ", " + KhoanmucchiphiId + " ," + KhoXuatId + "," + KhoNhanId + ",'" + masp + "',N'" + tensp + "',N'" + donvi + "','" + Solo + "','1' ) ";

            if (db.updateQueryReturnInt(query) <= 0)
            {
                msg = "3.Không thể cập nhật tài khoản kế toán " + query;
                return msg;
            }

            return msg;

        }

        public String Check_NgayNghiepVu_KeToan(clsDB db, String thang, String nam)
        {

            //CHECK THANG KHOA SO CO HOP LE HAY KHONG ( CHI DUOC CHOT SAU THANG KHOA SO + 1 )
            String query = "select THANGKS, NAM from ERP_KHOASOKETOAN order by NAM desc, THANGKS desc";
            String thangKS = "12";
            String namKS = "2016";
            DataTable rsCheck = db.getDataTable(query);

            for (int index = 0; index < rsCheck.Rows.Count; index++)
            {
                thangKS = rsCheck.Rows[index]["THANGKS"].ToString();
                namKS = rsCheck.Rows[index]["NAM"].ToString();
            }
            rsCheck.Clone();

            if ((chuyenInt(nam) < chuyenInt(namKS)) || ((chuyenInt(nam) == chuyenInt(namKS)) && (chuyenInt(thang) <= chuyenInt(thangKS))))
            {
                return "Không được thực hiện nghiệp vụ kế toán trong thời gian đã khóa sổ";
            }

            return "";
        }

        public String getLoaiKho(String khoid, clsDB db)
        {
            try
            {
                String query = "select  ISNULL(LOAIKHO,'') AS LOAI  from ERP_KHOTT where PK_SEQ  IN (" + khoid + ") ";
                
                DataTable rs = db.getDataTable(query);
                String loaikho = "";
                for (int index = 0; index < rs.Rows.Count; index ++ )
                {
                    loaikho = rs.Rows[index]["LOAI"].ToString();
                }
                rs.Clone();
                
                return loaikho;
            }
            catch (Exception er)
            {
                return "";
            }
        }


       

    }
    
}