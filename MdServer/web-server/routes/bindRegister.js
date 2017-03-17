/**
 *
 * 使用手机/邮箱绑定注册账号的逻辑
 */
var logger = require('../service/logger.js').logger;
var CODE = require('../../shared/code');
var CONST = require('../../shared/const');
var dbService = require('../service/dbService.js');

/*
 * ==================================== 使用邮箱注册 =============================================
 * */
exports.mailRegister = function(req, res) {
  var username = req.body.username;                         //用户名，即邮箱
  var password = req.body.password;                         //密码
  var regEx = /[a-zA-Z0-9.]+@[a-zA-Z0-9_]+?\.com/;          //匹配邮箱格式的正则
  var deviceID = req.body.deviceID || username;             //设备id
  var registerType = CONST.REGISTER_TYPE.MAIL;             //注册类型

  if(!username || !password || !regEx.test(username)) {
    res.send({code: CODE.LOGIN.REG_ILLEGAL});
    return;
  }

  // 先去账号库查找看这个账号是否已被注册
  dbService.selectUserinfo(username, function(userinfo) {
    if(!userinfo) {
      console.log('500 error');
      res.send({code: CODE.FAILED});
      return;
    }
    if(userinfo.length) {
      console.log('already exists');
      res.send({code: CODE.LOGIN.ALREADY_EXIST});
      return;
    }

    // 若账号未被注册，则写库注册
    dbService.registerUserinfo(username, password, deviceID, registerType, function(result) {
      if(!result) {
        console.log('500 error');
        res.send({code: CODE.FAILED});
        return;
      }
      console.log('success');
      res.send({code: CODE.SUCCESS});
    });
  });
};


/*
* ============================ 使用手机号注册 =======================================
* */
exports.phoneRegister = function(req, res) {

};