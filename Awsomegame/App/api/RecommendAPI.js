
import URLConf from './URLConf';
import Secret from '../utils/Secret';
import {getToken} from '../utils/Secret';
import Md5 from '../utils/Md5';
import PoplarEnv from '../utils/PoplarEnv';

export function getRecommendUsers(callback) {

  var path =  URLConf.API_HOST + '/recommend/users';
  var sign = Md5.hex_md5(path.replace(URLConf.APP_SERVER_HOST, '') + '?ts=123456&'+Secret.secret);
  console.log('sign:' + sign);
  var url = path+'?ts=123456&sign=' + sign;
  var options = {
    method: 'GET',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    },
  };

  fetch(url, options)
    .then((response) => response.json())
    .then((responseData) => {
      console.log(responseData);
      if(responseData.status == PoplarEnv.dic.SUCCESS) {
          callback(true, responseData.users);
      } else {
        callback(false, 'error');
      }
    })
    .done();
}

export function getRecommendTags(callback) {

  var path =  URLConf.API_HOST + '/recommend/tags';
  var sign = Md5.hex_md5(path.replace(URLConf.APP_SERVER_HOST, '') + '?ts=123456&'+Secret.secret);
  console.log('sign:' + sign);
  var url = path+'?ts=123456&sign=' + sign;
  var options = {
    method: 'GET',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    },
  };

  fetch(url, options)
    .then((response) => response.json())
    .then((responseData) => {
      console.log(responseData);
      if(responseData.status == PoplarEnv.dic.SUCCESS) {
          callback(true, responseData.tags);
      } else {
        callback(false, 'error');
      }
    })
    .done();

}
