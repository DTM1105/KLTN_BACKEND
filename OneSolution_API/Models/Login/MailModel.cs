using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using OneSolution_API.Models.kl;
using System.Data;

namespace OneSolution_API.Models.Login
{
    public class MailModel
    {
        static string  from_mail { get; set; } = "phucdpb@gmail.com";
        static string password { get; set; } = "xifn mxbg ugqs qnqr";

        public static Boolean sendMail(userModel userModel,int maxn)
        {
            try
            {
                MailMessage mail = new MailMessage(from_mail, userModel.email);
                mail.Subject = "Xác nhận đăng kí tài khoản";
                String thu = $@" <h3>{userModel.name} thân mến ✔ </h3>
                        <p> Đây là email tự động được gửi từ hệ thống đặt lịch khám bệnh TADA nhằm xác nhận việc đăng kí tài khoản của bạn trên nền tảng của chúng tôi</p>
                        <b> Đây là mã xác nhận đăng kí tài khoản của bạn: {maxn}</b>
                        <p> Trân trọng </p>";
                mail.Body = thu;
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                // Set the SMTP port and credentials (for Gmail)
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(from_mail, password);
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static Boolean sendMailtoPatient(userModel userModel, ScheduleModel schedule, ClinicModel cli,String code,int stt,String patientName)
        {
            try
            {
                String name = "";
                if (String.IsNullOrEmpty(patientName))
                {
                    name = userModel.name;
                }
                else
                {
                    name = patientName;
                }
                MailMessage mail = new MailMessage(from_mail, userModel.email);
                mail.Subject = "Thông báo thông tin lịch hẹn";
                String thu = $@" <h3>{name} thân mến ✔ </h3>
                        <p> Đây là email tự động được gửi từ hệ thống đặt lịch khám bệnh TADA nhằm thông báo kết quả đặt lịch khám bệnh của bạn </p>
                        <div> <b> Bạn đã đặt thành công lịch hẹn tại phòng khám {cli.name} vào buổi {schedule.timedt} ngày {schedule.date} </b></div>
                        <div><p> Mã khám bệnh của bạn là <b>{code}</b></p></div>
                        <div><b> Số thứ tự là {stt}</b></div>
                        <p> Trân trọng </p>";
                mail.Body = thu;
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                // Set the SMTP port and credentials (for Gmail)
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(from_mail, password);
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static Boolean sendMailtoPatientByAdmin(String email,String name, ScheduleModel schedule, ClinicModel cli, String code, int stt)
        {
            try
            {
                MailMessage mail = new MailMessage(from_mail, email);
                mail.Subject = "Thông báo thông tin lịch hẹn";
                String thu = $@" <h3>{name} thân mến ✔ </h3>
                        <p> Đây là email tự động được gửi từ hệ thống đặt lịch khám bệnh TADA nhằm thông báo kết quả đặt lịch khám bệnh của bạn </p>
                        <div> <b> Bạn đã đặt thành công lịch hẹn tại phòng khám {cli.name} vào buổi {schedule.timedt} ngày {schedule.date} </b></div>
                        <div><p> Mã khám bệnh của bạn là <b>{code}</b></p></div>
                        <div><b> Số thứ tự là {stt}</b></div>
                        <p> Trân trọng </p>";
                mail.Body = thu;
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                // Set the SMTP port and credentials (for Gmail)
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(from_mail, password);
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static Boolean sendMailbyAdmin(String email, String name,String password1)
        {
            try
            {
                MailMessage mail = new MailMessage(from_mail, email);
                mail.Subject = "Thông báo xác nhận đăng kí tài khoản";
                String thu = $@" <h3>{name} thân mến ✔ </h3>
                        <p> Đây là email tự động được gửi từ hệ thống đặt lịch khám bệnh TADA nhằm thông báo bạn đã tạo thành công tài khoản trên hệ thống của chúng tôi</p>
                        <p> Đây là tài khoản đăng nhập của bạn: <br>{email}</br></p>
                        <p> Đây là mật khẩu đăng nhập của bạn: <br>{password1}</br></p>
                        <p> Trân trọng </p>";
                mail.Body = thu;
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                // Set the SMTP port and credentials (for Gmail)
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(from_mail, password);
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static Boolean sendMailBenhAn(userModel patient,HistoryModel history, userModel dt,ClinicModel cli,String code,String ngaykham,String content)
        {
            try
            {
                MailMessage mail = new MailMessage(from_mail, patient.email);
                mail.Subject = "Thông báo thông tin đơn thuốc";
                String thu = "";
                if(content.Length == 0)
                {
                    thu = $@" <h3>{patient.name} thân mến ✔ </h3>
                        <p> Đây là email tự động được gửi từ hệ thống đặt lịch khám bệnh TADA nhằm thông báo về đơn thuốc của bạn</p>
                        <div> <b> Bạn đã khám bệnh thành công tại phòng khám {cli.name} vào ngày {ngaykham} </b></div>
                        <div>Chuẩn đoán của bác sĩ: {history.diagnostic}</div>
                        <p> Trân trọng </p>";
                }
                else
                {
                    thu = $@" <h3>{patient.name} thân mến ✔ </h3>
                        <p> Đây là email tự động được gửi từ hệ thống đặt lịch khám bệnh TADA nhằm thông báo về đơn thuốc của bạn</p>
                        <div> <b> Bạn đã khám bệnh thành công tại phòng khám {cli.name} vào ngày {ngaykham} </b></div>
                        <div>Chuẩn đoán của bác sĩ: {history.diagnostic}</div>
                        <div><p> Mã đơn thuốc của bạn là <b>{code}</b></p></div>
                        <div>Chi tiết đơn thuốc: </div>
                        {content}
                        <p> Trân trọng </p>";
                }
                
                mail.Body = thu;
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                // Set the SMTP port and credentials (for Gmail)
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(from_mail, password);
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static Boolean sendMailChangeSchedule(DataRow row)
        {
            try
            {
                MailMessage mail = new MailMessage(from_mail, row["mailPatient"].ToString());
                mail.Subject = "Thông báo thông tin thay đổi bác sĩ khám bệnh";
                String thu = $@" <h3>{row["patientName"]} thân mến ✔ </h3>
                        <p> Đây là email tự động được gửi từ hệ thống đặt lịch khám bệnh TADA nhằm thông báo thay đổi bác sĩ của bạn </p>
                        <div> <b> Lịch hẹn {row["bookingcode"]} vào buỏi {row["timedt"]} ___  {row["date"]} tại phòng khám {row["clinicName"]} đã được thay đổi bác sĩ</b></div>
                        <div><p> Từ bác sĩ {row["dtName"]} thành bác sĩ {row["todoctorName"]}</p></div>
                        <div><b> Lý do của bác sĩ là {row["reason"]}</b></div>
                        <p> Mong bạn thông cảm đến chúng tôi </p>
                        <p> Trân trọng </p>";

                mail.Body = thu;
                mail.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                // Set the SMTP port and credentials (for Gmail)
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(from_mail, password);
                smtpClient.EnableSsl = true;

                // Send the email
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}