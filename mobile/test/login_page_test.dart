import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/auth_repository.dart';
import 'package:tan_erp_field/core/auth/session_storage.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/gen/app_localizations.dart';
import 'package:tan_erp_field/core/theme/app_theme.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';
import 'package:tan_erp_field/features/auth/presentation/login_page.dart';

class FakeAuthRepository implements AuthRepository {
  SignInOutcome nextOutcome = const SignInCompleted();
  List<String> nextBackupCodes = const <String>[];
  AuthFailure? failureToThrow;

  @override
  bool get hasSession => false;

  @override
  Future<SignInOutcome> signIn({
    required String email,
    required String password,
  }) async {
    if (failureToThrow != null) throw failureToThrow!;
    return nextOutcome;
  }

  @override
  Future<List<String>> verifyTwoFactor({
    required String challengeToken,
    required String code,
  }) async {
    if (failureToThrow != null) throw failureToThrow!;
    return nextBackupCodes;
  }

  @override
  Future<String?> idToken({bool forceRefresh = false}) async => null;

  @override
  Future<void> signOut() async {}
}

Widget createLoginTestHarness({
  required FakeAuthRepository authRepo,
  bool isMockAuth = false,
}) {
  final config = AppConfig(
    flavor: Flavor.dev,
    apiBaseUrl: 'http://localhost',
    mockAuth: isMockAuth,
    firebase: const FirebaseConfig(
      apiKey: '',
      appId: '',
      messagingSenderId: '',
      projectId: '',
      storageBucket: '',
    ),
  );

  return ProviderScope(
    overrides: [
      appConfigProvider.overrideWithValue(config),
      authRepositoryProvider.overrideWithValue(authRepo),
      sessionStorageProvider.overrideWithValue(
        SessionStorage(),
      ),
    ],
    child: MaterialApp(
      locale: const Locale('th'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      theme: buildTheme(mode: Brightness.light),
      home: const LoginPage(),
    ),
  );
}

void main() {
  late FakeAuthRepository authRepo;

  setUp(() {
    authRepo = FakeAuthRepository();
  });

  group('LoginPage widget tests', () {
    testWidgets('renders email and password fields in normal auth mode',
        (tester) async {
      await tester.pumpWidget(createLoginTestHarness(authRepo: authRepo));
      await tester.pumpAndSettle();

      expect(find.byType(ErpTextField), findsNWidgets(2));
      expect(find.byType(ErpButton), findsOneWidget);
      expect(find.text('name@company.com'), findsOneWidget); // placeholder
    });

    testWidgets('renders uid field and mock banner in mock auth mode',
        (tester) async {
      await tester.pumpWidget(
        createLoginTestHarness(authRepo: authRepo, isMockAuth: true),
      );
      await tester.pumpAndSettle();

      expect(find.byType(ErpTextField), findsOneWidget);
      expect(find.text('foundation-user-test-only'), findsOneWidget);
    });

    testWidgets(
        'transitions to 2FA verification view on SignInTwoFactorRequired',
        (tester) async {
      authRepo.nextOutcome = const SignInTwoFactorRequired(
        challengeToken: 'test-challenge-123',
      );

      await tester.pumpWidget(createLoginTestHarness(authRepo: authRepo));
      await tester.pumpAndSettle();

      // Fill valid email & password
      await tester.enterText(
        find.byType(TextField).at(0),
        'user@example.com',
      );
      await tester.enterText(
        find.byType(TextField).at(1),
        'Password123!',
      );
      await tester.pumpAndSettle();

      // Submit
      await tester.tap(find.byType(ErpButton));
      await tester.pumpAndSettle();

      // Should now show 2FA verify view
      expect(find.byType(ErpTextField), findsOneWidget);
      expect(find.byType(ErpButton), findsNWidgets(2)); // Verify & Back buttons

      // Tap back button
      await tester.tap(find.byType(ErpButton).at(1));
      await tester.pumpAndSettle();

      // Should return to credentials view
      expect(find.byType(ErpTextField), findsNWidgets(2));
    });

    testWidgets(
        'transitions to 2FA setup view on SignInTwoFactorSetupRequired',
        (tester) async {
      authRepo.nextOutcome = const SignInTwoFactorSetupRequired(
        challengeToken: 'setup-challenge-456',
        secretKey: 'JBSWY3DPEHPK3PXP',
        otpAuthUri: 'otpauth://totp/TAN:test@test.com?secret=KEY',
      );

      await tester.pumpWidget(createLoginTestHarness(authRepo: authRepo));
      await tester.pumpAndSettle();

      // Fill credentials
      await tester.enterText(
        find.byType(TextField).at(0),
        'user@example.com',
      );
      await tester.enterText(
        find.byType(TextField).at(1),
        'Password123!',
      );
      await tester.pumpAndSettle();

      // Submit
      await tester.tap(find.byType(ErpButton));
      await tester.pumpAndSettle();

      // Should show 2FA Setup view with ErpCard containing secret key
      expect(find.byType(ErpCard), findsOneWidget);
      expect(find.text('JBSWY3DPEHPK3PXP'), findsOneWidget);
      expect(find.byType(ErpTextField), findsOneWidget);
    });

    testWidgets(
        'shows backup codes display view when completing setup',
        (tester) async {
      authRepo
        ..nextOutcome = const SignInTwoFactorSetupRequired(
          challengeToken: 'setup-challenge-456',
          secretKey: 'JBSWY3DPEHPK3PXP',
          otpAuthUri: 'otpauth://totp/TAN:test@test.com?secret=KEY',
        )
        ..nextBackupCodes = const ['CODE-1111', 'CODE-2222', 'CODE-3333'];

      await tester.pumpWidget(createLoginTestHarness(authRepo: authRepo));
      await tester.pumpAndSettle();

      // Fill credentials
      await tester.enterText(
        find.byType(TextField).at(0),
        'user@example.com',
      );
      await tester.enterText(
        find.byType(TextField).at(1),
        'Password123!',
      );
      await tester.pumpAndSettle();
      await tester.tap(find.byType(ErpButton));
      await tester.pumpAndSettle();

      // In setup view, enter 6-digit code
      await tester.enterText(find.byType(TextField), '123456');
      await tester.pumpAndSettle();

      // Submit code (the primary ErpButton is the first action)
      await tester.tap(
        find.widgetWithText(ErpButton, 'ยืนยันและเปิดใช้งาน 2FA'),
      );
      await tester.pumpAndSettle();

      // Should transition to backup codes view
      expect(find.text('CODE-1111'), findsOneWidget);
      expect(find.text('CODE-2222'), findsOneWidget);
      expect(find.text('CODE-3333'), findsOneWidget);
    });

    testWidgets('shows error alert on invalid credentials', (tester) async {
      authRepo.failureToThrow = const InvalidCredentialsFailure();

      await tester.pumpWidget(createLoginTestHarness(authRepo: authRepo));
      await tester.pumpAndSettle();

      // Fill credentials
      await tester.enterText(
        find.byType(TextField).at(0),
        'bad@example.com',
      );
      await tester.enterText(
        find.byType(TextField).at(1),
        'wrongpassword',
      );
      await tester.pumpAndSettle();

      await tester.tap(find.byType(ErpButton));
      await tester.pumpAndSettle();

      // Expect error alert
      expect(find.text('อีเมลหรือรหัสผ่านไม่ถูกต้อง'), findsOneWidget);
    });
  });
}
