module.exports = {
    OK: 200,
    FAILED: 500,
    LOGIN: {
        LOGIN_ILLEGAL:          600,                             //�û��������벻��ȷ
        NO_USER:                601,                             //
        ERR_PASSWORD:           602,
        //ע��ʱ��һЩ״̬��
        REG_ILLEGAL:            700,                            //��������ȷ���û���������
        ALREADY_EXIST:          701,                            //���˺��Ѵ���
        REJECT:                 702                             //�������дʻ�
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