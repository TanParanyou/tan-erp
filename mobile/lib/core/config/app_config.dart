import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

enum Flavor { dev, staging, prod }

/// Firebase options injected per flavor (no google-services.json / plist is
/// committed). Empty values mean "not configured".
@immutable
class FirebaseConfig {
  const FirebaseConfig({
    required this.apiKey,
    required this.appId,
    required this.messagingSenderId,
    required this.projectId,
    required this.storageBucket,
  });

  final String apiKey;
  final String appId;
  final String messagingSenderId;
  final String projectId;
  final String storageBucket;

  bool get isConfigured =>
      apiKey.isNotEmpty &&
      appId.isNotEmpty &&
      messagingSenderId.isNotEmpty &&
      projectId.isNotEmpty;
}

/// The flavor comes from the `main_<flavor>.dart` entry point; other values from `--dart-define-from-file=config/<flavor>.json`.
@immutable
class AppConfig {
  const AppConfig({
    required this.flavor,
    required this.apiBaseUrl,
    required this.firebase,
  });

  factory AppConfig.fromEnvironment(Flavor flavor) {
    const appIdAndroid = String.fromEnvironment('FIREBASE_APP_ID_ANDROID');
    const appIdIos = String.fromEnvironment('FIREBASE_APP_ID_IOS');
    return AppConfig(
      flavor: flavor,
      apiBaseUrl: const String.fromEnvironment('API_BASE_URL'),
      firebase: FirebaseConfig(
        apiKey: const String.fromEnvironment('FIREBASE_API_KEY'),
        appId: defaultTargetPlatform == TargetPlatform.iOS
            ? appIdIos
            : appIdAndroid,
        messagingSenderId: const String.fromEnvironment(
          'FIREBASE_MESSAGING_SENDER_ID',
        ),
        projectId: const String.fromEnvironment('FIREBASE_PROJECT_ID'),
        storageBucket: const String.fromEnvironment('FIREBASE_STORAGE_BUCKET'),
      ),
    );
  }

  final Flavor flavor;
  final String apiBaseUrl;
  final FirebaseConfig firebase;

  bool get isDev => flavor == Flavor.dev;
}

/// Overridden in bootstrap with the real config.
final appConfigProvider = Provider<AppConfig>(
  (ref) => throw UnimplementedError('appConfigProvider must be overridden'),
);
