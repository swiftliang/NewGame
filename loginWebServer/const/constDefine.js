/**
 * 
 * 定义代码中用到的一些常量
 */
module.exports = {
  LISTEN_PORT:              6688,                           //服务器监听的端口号

  //返回状态码定义
  CODE: {
    // 通用状态码
    SUCCESS:                200,                            //成功
    FAILED:                 500,                            //失败
    DATE_ERR:               404,                            //请求数据不正确

    //登录时的一些状态码
    LOGIN_ILLEGAL:          600,                            //用户名或密码不正确
    NO_USER:                601,                            //不存在此用户名
    ERR_PASSWORD:           602,                            //密码错误

    //注册时的一些状态码
    REG_ILLEGAL:            700,                            //请输入正确的用户名或密码
    ALREADY_EXIST:          701,                            //该账号已存在
    REJECT:                 702                             //包含敏感词汇
  },

  //注册类型（区分本地注册、渠道注册）
  REGISTER_TYPE: {
    LOCAL:                  'local',                        //本地注册
    QUICK:                  'quick',                        //快速注册
    MAIL:                   'mail'                          //邮箱注册
  },

  //token加密秘钥
  TOKEN_SECRET:             'MDServer_session_secret'
};