import { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.efams.mobile',
  appName: 'efams',
  webDir: 'dist/apps/mobile-field',
  server: {
    androidScheme: 'https'
  }
};

export default config;
