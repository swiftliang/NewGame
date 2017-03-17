/**
 * 
 * token的加密解密
 */
var crypto = require('crypto');

/*
 * token加密
 * 使用玩家'uid*时间戳'的格式加密
 * */
module.exports.create = function(uid, timestamp, pwd) {
  var msg = uid + '$' + timestamp;
  var cipher = crypto.createCipher('aes256', pwd);
  var encrypt = cipher.update(msg, 'utf8', 'hex');
  encrypt += cipher.final('hex');
  return encrypt;
};

/*
 * token解密
 * */
module.exports.parse = function(token, pwd) {
  var decipher = crypto.createDecipher('aes256', pwd);
  var decrypt;
  try{
    decrypt = decipher.update(token, 'hex', 'utf8');
    decrypt += decipher.final('utf8');
  } catch(err) {
    console.error('fail to decrypt token');
    return null;
  }

  var result = decrypt.split('$');
  if(result.length !== 2) {
    // 非法的token
    return null;
  }
  return {uid: result[0], timestamp: Number(result[1])};
};

/*
* 玩家密码加密
* */
module.exports.encryptPassword = function() {};

/*
 *  md5加密
 * */
module.exports.md5Value = function(data) {
  var md5 = crypto.createHash('md5');
  md5.update(data);
  return md5.digest('hex');
};