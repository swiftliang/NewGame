module.exports = {
    //ע�����ͣ����ֱ���ע�ᡢ����ע�ᣩ
    REGISTER_TYPE: {
        LOCAL:                  'local',                        //����ע��
        QUICK:                  'quick',                        //����ע��
        MAIL:                   'mail'                          //����ע��
    },

    //token������Կ
    TOKEN_SECRET:             'MDServer_session_secret',
    TOKEN_EXPIRE:              6 * 60 * 60 * 1000
};