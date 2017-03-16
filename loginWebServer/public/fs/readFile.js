/**
 * Created by baoyan on 2016/4/6 0006.
 * 读取文件，做匹配敏感词等操作
 */
var fs = require('fs');
var path = require('path');
var logger = require('../../service/logger.js').logger;

var readFile = module.exports;

/*
*  检查是否包含敏感词汇表(shield.txt)中的词汇
* */
readFile.checkIllegalWord = function(word, callback) {
  fs.readFile(path.join(__dirname, 'shield.txt'), {encoding: 'utf8', flag: 'r'}, function(err, data) {
    if(err) {
      logger.error('checkIllegalWord error: ' + err);
      callback(false);
      return;
    }
    var reg = eval("/(、){1}"+word+"(、){1}/");
    var result = reg.test(data);
    callback(result);
  });
};