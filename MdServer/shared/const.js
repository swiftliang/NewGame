module.exports = {
    //注册类型（区分本地注册、渠道注册）
    REGISTER_TYPE: {
        LOCAL:                  'local',                        //本地注册
        QUICK:                  'quick',                        //快速注册
        MAIL:                   'mail'                          //邮箱注册
    },

    //token加密秘钥
    TOKEN_SECRET:             'MDServer_session_secret',
    TOKEN_EXPIRE:              6 * 60 * 60 * 1000
};