using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Combination0608.Models {
    public class CheckEmp {

        public string Title { get; set; }   //Email 標題
        public string Body { get; set; }    //Email 內容

        PCGEntities db = new PCGEntities();

        public void Check(string Email) {   //負責搜尋並檢查主管是否維護員工資料
            List<string> MailList = new List<string>();

            //var query = (from A in db.Administrators
            //             where A.EmployeeID == 53 && A.Role == "0"
            //             select A.Email).ToList();

            //if (query.Any()) {  //檢查 query 是否為 null
            //    foreach (var item in query) {
            //        MailList.Add(item);
            //    }

                MailList.Add(Email);
                Title = "2016年六月份_員工出勤維護通知";
                Body = string.Format("<!DOCTYPE html><html><head><meta http-equiv='Content - Type' content='text / html; charset = utf - 8'/> <title>出勤維護通知排程</title><meta charset='utf - 8' /></head><body> <h1> <strong>Dear 主管</strong> </h1> <p> <span style='font - size:14px; '>您上個月的員工出勤紀錄尚未維護</span> </p> <p> <span style='font - size:14px; '>請務必在 15 號前維護完成</span> </p> <p> <span style='font - size:14px; '>感謝。</span> </p> <br /> <div> <a href='http://www.pouchen.com/'><img src='http://i.imgur.com/pCLHCYU.png?1' style='width: 198px; height: 58px;' /></a> </div></body></html>", "{0}");
            //Body = string.Format("Dear 主管<br/>您上個月的員工出勤紀錄尚未維護<br/>請務必在 15 號前維護完成<br/>感謝。","{0}");
            SendMailByGmail(MailList, Title, Body); //呼叫 SendMailByGmail
            //}
        }

        //Check 帶入參數
        //public void Check(string mailtitle, string mailbody) {   //負責搜尋並檢查主管是否維護員工資料
        //    List<string> MailList = new List<string>();

        //    var query = (from A in db.Administrators
        //                 where A.EmployeeID == 53 && A.Role == "0"
        //                 select A.Email).ToList();

        //    if (query.Any()) {  //檢查 query 是否為 null
        //        foreach (var item in query) {
        //            MailList.Add(item);
        //        }
        //        Title = mailtitle;
        //        Body = mailbody;
        //        SendMailByGmail(MailList, Title, Body); //呼叫 SendMailByGmail
        //    }
        //}


        //測試用email --> taipeibigegg@gmail.com
        //寄 Email  方法
        public void SendMailByGmail(List<string> MailList, string Title, string Body) {
            MailMessage msg = new MailMessage();

            msg.To.Add(string.Join(",", MailList.ToArray()));
            msg.From = new MailAddress("pouchenemp@gmail.com", "寶成國際集團", Encoding.UTF8);
            msg.Subject = Title;    //郵件標題
            msg.SubjectEncoding = Encoding.UTF8;    //郵件標題編碼
            //郵件內容
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = Encoding.UTF8;
            msg.Priority = MailPriority.Normal;

            //#region 其它 Host
            /*
             *  outlook.com smtp.live.com port:25
             *  yahoo smtp.mail.yahoo.com.tw port:465
            */
            ////#endregion
            ////建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port 
            //SmtpClient mysmtp = new SmtpClient("smtp.gmail.com", 587);
            ////設定寄件者的郵件帳號密碼
            //mysmtp.Credentials = new NetworkCredential("pouchenemp@gmail.com", "pouchen@web");
            ////Gmial 的 smtp 使用 SSL
            //mysmtp.EnableSsl = true;
            //mysmtp.Send(msg);
        }
    }
}