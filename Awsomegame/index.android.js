'use strict';

import React from 'react';
import {
  AppRegistry,
  Navigator,
  View
} from 'react-native';

import App from './App/App';
import LoginRegPage from './App/LoginRegPage';

export default class Awsomegame extends Component {
  render() {
    return (
      /*<Navigator
          initialRoute={{ component: App }}
          configureScene={(route) => {
              return Navigator.SceneConfigs.FloatFromRight;
          }}
          renderScene={(route, navigator) => {
            let Component = route.component;
              return <Component {...route.params} navigator={navigator} />
          }} />*/
      <View>
        <LoginRegPage />
      </View>  
    );
  }
}
AppRegistry.registerComponent('Awsomegame', () => Awsomegame);
