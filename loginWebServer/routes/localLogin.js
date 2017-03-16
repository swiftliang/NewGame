/**
 * 
 * 本地(非其它渠道)登录/注册的逻辑
 */
var CONSTDEFINE = require('../const/constDefine.js');
var serverConfig = require('../config/server.json');
var dbService = require('../service/dbService.js');
var logger = require('../service/logger.js').logger;
var readFile = require('../public/fs/readFile.js');
var tokenService = require('../service/tokenService.js');
var crypto = require('../service/crypto');

/*
*  ===========================  本地登录验证处理逻辑  ===========================
* */
exports.login = function(req, res){
  var userName = req.body.userName;
  var password = req.body.password;
  // 判断请求的账户和密码字段是否有效
  if(!userName || !password) {
    console.log('用户名或密码不正确');
    res.send({code: CONSTDEFINE.CODE.LOGIN_ILLEGAL});
    return;
  }
  // 去玩家账号库里面查找此账号信息
  dbService.selectUserinfo(userName, function(userinfo) {
    // 查库出错，服务器内部错误
    if(!userinfo) {
      console.log('服务器内部错误，查库出错');
      res.send({code: CONSTDEFINE.CODE.FAILED});
      return;
    }
    // 没查找到此账户信息
    if(!userinfo.length) {
      console.log('没有此用户名');
      res.send({code: CONSTDEFINE.CODE.NO_USER});
      return;
    }
    // 密码不正确
    if(crypto.md5(password) != userinfo[0].password) {
      console.log('用户名密码不匹配');
      res.send({code: CONSTDEFINE.CODE.ERR_PASSWORD});
      return;
    }
    // 验证成功，返回gate服务器地址
    console.log('登录成功');
    res.send({
      code: CONSTDEFINE.CODE.SUCCESS,
      uid: userinfo[0].id,
      token: tokenService.create(userinfo[0].id, Date.now(), CONSTDEFINE.TOKEN_SECRET),
      gateHost: serverConfig.gateServer.gateHost,
      gatePort: serverConfig.gateServer.gatePort
    });
    // 登陆成功，更新玩家登录信息
    dbService.updateUserLoginTime(userName);
  });
};

/*
*  ===============================  本地注册处理逻辑  =====================================
* */
exports.register = function(req, res) {
  var userName = req.body.userName;
  var password = req.body.password;
  // 验证请求的账户和密码字段有效性
  if(!userName || !password) {
    console.log('请输入正确的用户名或密码');
    res.send({code: CONSTDEFINE.CODE.REG_ILLEGAL});
    return;
  }

  // 检查是否包含敏感词汇
  readFile.checkIllegalWord(userName, function(result) {
    if(result) {
      console.log('用户名包含敏感词汇');
      res.send({code: CONSTDEFINE.CODE.REJECT});
      return;
    }
    // 先去账号库查找看这个账号是否已被注册
    dbService.selectUserinfo(userName, function(userinfo) {
      if(!userinfo) {
        console.log('服务器内部错误,查库失败');
        res.send({code: CONSTDEFINE.CODE.FAILED});
        return;
      }
      if(userinfo.length) {
        console.log('该账号已存在');
        res.send({code: CONSTDEFINE.CODE.ALREADY_EXIST});
        return;
      }

      var deviceID = req.body.deviceID || userName;
      var registerType = CONSTDEFINE.REGISTER_TYPE.LOCAL;
      // 若账号未被注册，则写库注册
      dbService.registerUserinfo(userName, password, deviceID, registerType, function(result) {
        if(!result) {
          console.log('服务器内部错误，写库失败');
          res.send({code: CONSTDEFINE.CODE.FAILED});
          return;
        }
        console.log('注册成功');
        res.send({code: CONSTDEFINE.CODE.SUCCESS});
      });
    });
  });
};

/*
* ================================  快速注册处理逻辑  =======================================
* */
exports.quickRegister = function(req, res) {
  //var deviceID = req.body.deviceID;                         //获取玩家设备id
  //if(!deviceID) {
  //  console.log('缺少设备ID');
  //  res.send({code: CONSTDEFINE.CODE.REG_ILLEGAL});
  //  return;
  //}
  //
  //var quickuserName = deviceID;                             //设备ID作为用户名
  //var quickPassword = tokenService.md5Value(deviceID);      //设备ID的md5作为密码
  //var registerType = CONSTDEFINE.REGISTER_TYPE.QUICK;       //注册类型：快速注册
  //
  //// 先去账号库查找看这个账号是否已被注册
  //dbService.selectUserinfo(quickuserName, function(userinfo) {
  //  if(!userinfo) {
  //    console.log('服务器内部错误,查库失败');
  //    res.send({code: CONSTDEFINE.CODE.FAILED});
  //    return;
  //  }
  //  if(userinfo.length) {
  //    console.log('该账号已存在');
  //    res.send({code: CONSTDEFINE.CODE.ALREADY_EXIST});
  //    return;
  //  }
  //
  //  // 若账号未被注册，则写库注册
  //  dbService.registerUserinfo(quickuserName, quickPassword, deviceID, registerType, function(result) {
  //    if(!result) {
  //      console.log('服务器内部错误，写库失败');
  //      res.send({code: CONSTDEFINE.CODE.FAILED});
  //      return;
  //    }
  //    console.log('注册成功');
  //    res.send({code: CONSTDEFINE.CODE.SUCCESS});
  //  });
  //});


//~~~~~~~~~~~~~~~~~~~ 测试使用逻辑 ~~~~~~~~~~~~~~~~~~~~~~
  var deviceID = req.body.deviceID;                         //获取玩家设备id
  if(!deviceID) {
    console.log('req data error');
    res.send({code: CONSTDEFINE.CODE.REG_ILLEGAL});
    return;
  }
  var registerType = CONSTDEFINE.REGISTER_TYPE.QUICK;       //注册类型：快速注册
  var quickPassword = '111111';                             //快速注册的密码(dmeo阶段测试使用)

  dbService.quickRegister(quickPassword, deviceID, registerType, function(result) {
    if(!result) {
      console.log('500 error');
      res.send({code: CONSTDEFINE.CODE.FAILED});
      return;
    }

    console.log('quickRegister success');
    res.send({
      code: CONSTDEFINE.CODE.SUCCESS,
      userName: result.toString(),
      password: quickPassword
    })
  });
};
