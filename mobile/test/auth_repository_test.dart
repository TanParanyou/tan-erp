import 'dart:async';
import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/auth_repository.dart';
import 'package:tan_erp_field/core/auth/rest_auth_repository.dart';
import 'package:tan_erp_field/core/auth/session_storage.dart';
import 'package:tan_erp_field/core/network/interceptors.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  late FlutterSecureStorage secureStorage;
  late SessionStorage sessionStorage;
  late Dio dio;
  late AuthApi authApi;
  late RestAuthRepository repo;

  // Interceptor callback hook for tests
  FutureOr<Response<Object?>?> Function(
    RequestOptions options,
  )? onRequestHandler;

  setUp(() async {
    FlutterSecureStorage.setMockInitialValues({});
    secureStorage = const FlutterSecureStorage();
    sessionStorage = SessionStorage(storage: secureStorage);
    await sessionStorage.init();

    dio = Dio(BaseOptions(baseUrl: 'http://test'));
    dio.interceptors.add(ProblemDetailsInterceptor());
    dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          if (onRequestHandler != null) {
            final res = await onRequestHandler!(options);
            if (res != null) {
              return handler.resolve(res);
            }
          }
          handler.next(options);
        },
      ),
    );
    authApi = AuthApi(dio, standardSerializers);
    repo = RestAuthRepository(authApi: authApi, sessionStorage: sessionStorage);
  });

  group('RestAuthRepository - Login flows', () {
    test('successful login returns SignInCompleted and persists refresh token',
        () async {
      onRequestHandler = (options) {
        expect(options.path, equals('/api/v1/auth/login'));
        return Response<Map<String, dynamic>>(
          requestOptions: options,
          statusCode: 200,
          data: <String, dynamic>{
            'status': 'authenticated',
            'accessToken': 'jwt-access-123',
            'expiresAtUtc': DateTime.now()
                .toUtc()
                .add(const Duration(hours: 1))
                .toIso8601String(),
            'refreshToken': 'rt-token-456',
            'sessionId': 'session-789',
          },
        );
      };

      expect(repo.hasSession, isFalse);

      final outcome = await repo.signIn(
        email: 'test@example.com',
        password: 'Password123!',
      );

      expect(outcome, isA<SignInCompleted>());
      expect(repo.hasSession, isTrue);
      expect(await sessionStorage.readRefreshToken(), equals('rt-token-456'));
      expect(await sessionStorage.readSessionId(), equals('session-789'));
      expect(await repo.idToken(), equals('jwt-access-123'));
    });

    test('login with requires_two_factor returns SignInTwoFactorRequired',
        () async {
      onRequestHandler = (options) {
        return Response<Map<String, dynamic>>(
          requestOptions: options,
          statusCode: 200,
          data: <String, dynamic>{
            'status': 'requires_two_factor',
            'challengeToken': 'challenge-token-abc',
          },
        );
      };

      final outcome = await repo.signIn(
        email: 'test@example.com',
        password: 'Password123!',
      );

      expect(outcome, isA<SignInTwoFactorRequired>());
      final req = outcome as SignInTwoFactorRequired;
      expect(req.challengeToken, equals('challenge-token-abc'));
      expect(repo.hasSession, isFalse);
    });

    test(
      'login with requires_two_factor_setup returns '
      'SignInTwoFactorSetupRequired',
      () async {
      onRequestHandler = (options) {
        return Response<Map<String, dynamic>>(
          requestOptions: options,
          statusCode: 200,
          data: <String, dynamic>{
            'status': 'requires_two_factor_setup',
            'challengeToken': 'challenge-token-setup',
            'totpSetup': <String, dynamic>{
              'secretKey': 'JBSWY3DPEHPK3PXP',
              'otpAuthUri': 'otpauth://totp/TAN:user@test.com?secret=KEY',
            },
          },
        );
      };

      final outcome = await repo.signIn(
        email: 'test@example.com',
        password: 'Password123!',
      );

      expect(outcome, isA<SignInTwoFactorSetupRequired>());
      final req = outcome as SignInTwoFactorSetupRequired;
      expect(req.challengeToken, equals('challenge-token-setup'));
      expect(req.secretKey, equals('JBSWY3DPEHPK3PXP'));
      expect(
        req.otpAuthUri,
        equals('otpauth://totp/TAN:user@test.com?secret=KEY'),
      );
    });

    test('login maps INVALID_CREDENTIALS error code correctly', () async {
      onRequestHandler = (options) {
        throw DioException(
          requestOptions: options,
          response: Response<Map<String, dynamic>>(
            requestOptions: options,
            statusCode: 401,
            data: <String, dynamic>{
              'status': 401,
              'code': 'INVALID_CREDENTIALS',
              'title': 'Invalid credentials',
            },
          ),
        );
      };

      expect(
        () => repo.signIn(email: 'bad@example.com', password: 'wrong'),
        throwsA(isA<InvalidCredentialsFailure>()),
      );
    });

    test('login maps ACCOUNT_LOCKED_OUT error code correctly', () async {
      onRequestHandler = (options) {
        throw DioException(
          requestOptions: options,
          response: Response<Map<String, dynamic>>(
            requestOptions: options,
            statusCode: 403,
            data: <String, dynamic>{
              'status': 403,
              'code': 'ACCOUNT_LOCKED_OUT',
              'title': 'Locked out',
            },
          ),
        );
      };

      expect(
        () => repo.signIn(email: 'locked@example.com', password: 'any'),
        throwsA(isA<AccountLockedOutFailure>()),
      );
    });
  });

  group('RestAuthRepository - Two Factor Verification', () {
    test('verifyTwoFactor succeeds, stores tokens and returns backup codes',
        () async {
      onRequestHandler = (options) {
        expect(options.path, equals('/api/v1/auth/2fa/verify-login'));
        return Response<Map<String, dynamic>>(
          requestOptions: options,
          statusCode: 200,
          data: <String, dynamic>{
            'status': 'authenticated',
            'accessToken': 'jwt-after-2fa',
            'expiresAtUtc': DateTime.now()
                .toUtc()
                .add(const Duration(hours: 1))
                .toIso8601String(),
            'refreshToken': 'rt-after-2fa',
            'sessionId': 'session-2fa',
            'backupCodes': <String>['bc-1', 'bc-2'],
          },
        );
      };

      final backupCodes = await repo.verifyTwoFactor(
        challengeToken: 'valid-challenge',
        code: '123456',
      );

      expect(backupCodes, equals(<String>['bc-1', 'bc-2']));
      expect(repo.hasSession, isTrue);
      expect(await repo.idToken(), equals('jwt-after-2fa'));
      expect(await sessionStorage.readRefreshToken(), equals('rt-after-2fa'));
    });

    test('verifyTwoFactor maps TWO_FACTOR_CODE_INVALID correctly', () async {
      onRequestHandler = (options) {
        throw DioException(
          requestOptions: options,
          response: Response<Map<String, dynamic>>(
            requestOptions: options,
            statusCode: 400,
            data: <String, dynamic>{
              'status': 400,
              'code': 'TWO_FACTOR_CODE_INVALID',
            },
          ),
        );
      };

      expect(
        () => repo.verifyTwoFactor(
          challengeToken: 'valid-challenge',
          code: '000000',
        ),
        throwsA(isA<TwoFactorCodeInvalidFailure>()),
      );
    });
  });

  group('RestAuthRepository - Token refresh & Single-flight', () {
    test('idToken returns cached token when valid without network call',
        () async {
      var networkCalls = 0;
      onRequestHandler = (options) {
        networkCalls++;
        return Response<Map<String, dynamic>>(
          requestOptions: options,
          statusCode: 200,
          data: <String, dynamic>{
            'status': 'authenticated',
            'accessToken': 'valid-cached-token',
            'expiresAtUtc': DateTime.now()
                .toUtc()
                .add(const Duration(minutes: 10))
                .toIso8601String(),
            'refreshToken': 'rt-initial',
          },
        );
      };

      await repo.signIn(email: 'user@test.com', password: 'pass');
      expect(networkCalls, equals(1));

      final token1 = await repo.idToken();
      final token2 = await repo.idToken();
      expect(token1, equals('valid-cached-token'));
      expect(token2, equals('valid-cached-token'));
      expect(networkCalls, equals(1)); // No extra network calls
    });

    test(
      'idToken with forceRefresh performs single-flight refresh '
      'and rotates token',
      () async {
      var refreshCalls = 0;
      final completer = Completer<Response<Map<String, dynamic>>>();

      onRequestHandler = (options) {
        if (options.path == '/api/v1/auth/refresh') {
          refreshCalls++;
          return completer.future;
        }
        return Response<Map<String, dynamic>>(
          requestOptions: options,
          statusCode: 200,
          data: <String, dynamic>{
            'status': 'authenticated',
            'accessToken': 'old-token',
            'expiresAtUtc': DateTime.now()
                .toUtc()
                .add(const Duration(seconds: 10))
                .toIso8601String(),
            'refreshToken': 'rt-old',
          },
        );
      };

      await repo.signIn(email: 'user@test.com', password: 'pass');

      // Concurrent idToken calls needing refresh
      final future1 = repo.idToken(forceRefresh: true);
      final future2 = repo.idToken(forceRefresh: true);
      final future3 = repo.idToken(forceRefresh: true);

      await Future<void>.delayed(const Duration(milliseconds: 10));

      // Verify only 1 network call was initiated while in flight
      expect(refreshCalls, equals(1));

      // Resolve the refresh call
      completer.complete(
        Response<Map<String, dynamic>>(
          requestOptions: RequestOptions(path: '/api/v1/auth/refresh'),
          statusCode: 200,
          data: <String, dynamic>{
            'status': 'authenticated',
            'accessToken': 'new-rotated-token',
            'expiresAtUtc': DateTime.now()
                .toUtc()
                .add(const Duration(minutes: 15))
                .toIso8601String(),
            'refreshToken': 'rt-rotated-new',
            'sessionId': 'session-rotated',
          },
        ),
      );

      final tokens = await Future.wait([future1, future2, future3]);
      expect(tokens[0], equals('new-rotated-token'));
      expect(tokens[1], equals('new-rotated-token'));
      expect(tokens[2], equals('new-rotated-token'));
      expect(refreshCalls, equals(1));
      expect(
        await sessionStorage.readRefreshToken(),
        equals('rt-rotated-new'),
      );
    });

    test('401 on refresh clears stored refresh token and returns null',
        () async {
      await sessionStorage.writeRefreshToken('invalid-refresh-token');

      onRequestHandler = (options) {
        throw DioException(
          requestOptions: options,
          response: Response<Map<String, dynamic>>(
            requestOptions: options,
            statusCode: 401,
            data: <String, dynamic>{
              'status': 401,
              'code': 'AUTHENTICATION_INVALID',
            },
          ),
        );
      };

      final token = await repo.idToken(forceRefresh: true);
      expect(token, isNull);
      expect(await sessionStorage.readRefreshToken(), isNull);
      expect(repo.hasSession, isFalse);
    });
  });

  group('RestAuthRepository - Sign Out', () {
    test('signOut clears tokens and storage and calls logout endpoint',
        () async {
      var logoutCalled = false;
      await sessionStorage.writeRefreshToken('rt-logout-test');
      await sessionStorage.writeSessionId('sid-test');

      onRequestHandler = (options) {
        if (options.path == '/api/v1/auth/logout') {
          logoutCalled = true;
          return Response<void>(
            requestOptions: options,
            statusCode: 204,
          );
        }
        return null;
      };

      await repo.signOut();

      expect(logoutCalled, isTrue);
      expect(await sessionStorage.readRefreshToken(), isNull);
      expect(await sessionStorage.readSessionId(), isNull);
      expect(repo.hasSession, isFalse);
    });

    test('signOut succeeds even when network fails (offline logout)',
        () async {
      await sessionStorage.writeRefreshToken('rt-offline-test');

      onRequestHandler = (options) {
        throw DioException(
          requestOptions: options,
          type: DioExceptionType.connectionError,
          error: 'No internet connection',
        );
      };

      await repo.signOut();

      expect(await sessionStorage.readRefreshToken(), isNull);
      expect(repo.hasSession, isFalse);
    });
  });
}
