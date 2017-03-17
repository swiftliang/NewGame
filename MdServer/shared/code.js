module.exports = {
    OK: 200,
    FAILED: 500,
    LOGIN: {
        LOGIN_ILLEGAL:          600,                             //用户名或密码不正确
        NO_USER:                601,                             //
        ERR_PASSWORD:           602,
        //注册时的一些状态码
        REG_ILLEGAL:            700,                            //请输入正确的用户名或密码
        ALREADY_EXIST:          701,                            //该账号已存在
        REJECT:                 702                             //包含敏感词汇
    },
    ENTRY: {
        FA_TOKEN_INVALID: 1001,
        FA_TOKEN_EXPIRE: 1002,
        FA_USER_NOT_EXIST: 1003,
        FA_PLAYER_CREATE_FAILED: 1004
    },

    GATE: {
        FA_NO_SERVER_AVAILABLE: 2001
    }
};