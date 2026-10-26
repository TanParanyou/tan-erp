import 'dart:async';
import 'dart:ui';

import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_crashlytics/firebase_crashlytics.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:talker/talker.dart';
import 'package:tan_erp_field/app/app.dart';
import 'package:tan_erp_field/core/config/app_config.dart';

/// Shared entry for every flavor: Firebase (if configured), Crashlytics (dev),
/// then the app. Logging records codes and flavors only, never tokens or PII.
Future<void> bootstrap(Flavor flavor) async {
  WidgetsFlutterBinding.ensureInitialized();
  final talker = Talker();
  var config = AppConfig.fromEnvironment(flavor);

  if (config.firebase.isConfigured) {
    try {
      final f = config.firebase;
      await Firebase.initializeApp(
        options: FirebaseOptions(
          apiKey: f.apiKey,
          appId: f.appId,
          messagingSenderId: f.messagingSenderId,
          projectId: f.projectId,
          storageBucket: f.storageBucket.isEmpty ? null : f.storageBucket,
        ),
      );
      if (config.isDev) _enableCrashlytics();
    } on FirebaseException catch (e) {
      talker.error('Firebase init failed: ${e.code}');
      // Fall back to the "not configured" state instead of crashing.
      config = AppConfig(
        flavor: flavor,
        apiBaseUrl: config.apiBaseUrl,
        firebase: const FirebaseConfig(
          apiKey: '',
          appId: '',
          messagingSenderId: '',
          projectId: '',
          storageBucket: '',
        ),
      );
    }
  } else {
    talker.warning('Firebase not configured for ${flavor.name}');
  }

  runApp(
    ProviderScope(
      overrides: [appConfigProvider.overrideWithValue(config)],
      child: const App(),
    ),
  );
}

void _enableCrashlytics() {
  final crashlytics = FirebaseCrashlytics.instance;
  FlutterError.onError = crashlytics.recordFlutterFatalError;
  PlatformDispatcher.instance.onError = (error, stack) {
    unawaited(crashlytics.recordError(error, stack, fatal: true));
    return true;
  };
}
