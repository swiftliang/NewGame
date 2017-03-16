/**
 * 
 * 邮件发送验证的管理模块
 */
var nodemailer = require('nodemailer');
var logger = require('../service/logger.js').logger;

var mailService = module.exports;

mailService.sendMail = function(receiveMailBox, subject, html) {
  // 创建一个SMTP客户端，根据具体的发件邮箱进行配置
  var config = {
    host: 'smtp.163.com',
    port: 25,
    auth: {
      user: '18256973771@163.com',
      pass: 'BAOWCY11236129'
    }
  };

  // 创建一个SMTP客户端对象
  var transporter = nodemailer.createTransport(config);

  // 创建一个邮件对象
  var mail = {
    from: 'wanmei_fishing<18256973771@163.com>',
    subject: subject,
    to: mailBox,
    html: html
  };

  // 发送邮件
  transporter.sendMail(mail, function(err, data) {
    if(err) {
      logger.error('sendMail error: ' + err);
      return;
    }

    console.log(data);
  });
};