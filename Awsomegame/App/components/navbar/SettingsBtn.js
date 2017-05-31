'use strict'

import React from 'react';
import {
  Image,
  TouchableOpacity,
  StyleSheet
} from 'react-native';

export default class SettingsBtn extends React.Component{

  render() {
    return (
      <TouchableOpacity onPress={this.props.onPress} style={styles.container}>
        <Image
          source={require('../../../assets/img/settings.png')}
          style={[{ width: 18, height: 18, marginRight: 18, marginTop: 15},]}/>
      </TouchableOpacity>
    );
  }
}


var styles = StyleSheet.create({
  container: {

  },
});