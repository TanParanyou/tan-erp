//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/totp_setup_payload_dto.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/auth_user_summary_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'auth_result_response.g.dart';

/// AuthResultResponse
///
/// Properties:
/// * [status] 
/// * [accessToken] 
/// * [expiresAtUtc] 
/// * [refreshToken] 
/// * [sessionId] 
/// * [user] 
/// * [challengeToken] 
/// * [totpSetup] 
/// * [backupCodes] 
@BuiltValue()
abstract class AuthResultResponse implements Built<AuthResultResponse, AuthResultResponseBuilder> {
  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'accessToken')
  String? get accessToken;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  @BuiltValueField(wireName: r'refreshToken')
  String? get refreshToken;

  @BuiltValueField(wireName: r'sessionId')
  String? get sessionId;

  @BuiltValueField(wireName: r'user')
  AuthUserSummaryDto? get user;

  @BuiltValueField(wireName: r'challengeToken')
  String? get challengeToken;

  @BuiltValueField(wireName: r'totpSetup')
  TotpSetupPayloadDto? get totpSetup;

  @BuiltValueField(wireName: r'backupCodes')
  BuiltList<String>? get backupCodes;

  AuthResultResponse._();

  factory AuthResultResponse([void updates(AuthResultResponseBuilder b)]) = _$AuthResultResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AuthResultResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AuthResultResponse> get serializer => _$AuthResultResponseSerializer();
}

class _$AuthResultResponseSerializer implements PrimitiveSerializer<AuthResultResponse> {
  @override
  final Iterable<Type> types = const [AuthResultResponse, _$AuthResultResponse];

  @override
  final String wireName = r'AuthResultResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AuthResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.accessToken != null) {
      yield r'accessToken';
      yield serializers.serialize(
        object.accessToken,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expiresAtUtc != null) {
      yield r'expiresAtUtc';
      yield serializers.serialize(
        object.expiresAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.refreshToken != null) {
      yield r'refreshToken';
      yield serializers.serialize(
        object.refreshToken,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sessionId != null) {
      yield r'sessionId';
      yield serializers.serialize(
        object.sessionId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.user != null) {
      yield r'user';
      yield serializers.serialize(
        object.user,
        specifiedType: const FullType(AuthUserSummaryDto),
      );
    }
    if (object.challengeToken != null) {
      yield r'challengeToken';
      yield serializers.serialize(
        object.challengeToken,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.totpSetup != null) {
      yield r'totpSetup';
      yield serializers.serialize(
        object.totpSetup,
        specifiedType: const FullType(TotpSetupPayloadDto),
      );
    }
    if (object.backupCodes != null) {
      yield r'backupCodes';
      yield serializers.serialize(
        object.backupCodes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AuthResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AuthResultResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'accessToken':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.accessToken = valueDes;
          break;
        case r'expiresAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.expiresAtUtc = valueDes;
          break;
        case r'refreshToken':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.refreshToken = valueDes;
          break;
        case r'sessionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sessionId = valueDes;
          break;
        case r'user':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AuthUserSummaryDto),
          ) as AuthUserSummaryDto?;
          if (valueDes == null) continue;
          result.user.replace(valueDes);
          break;
        case r'challengeToken':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.challengeToken = valueDes;
          break;
        case r'totpSetup':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(TotpSetupPayloadDto),
          ) as TotpSetupPayloadDto?;
          if (valueDes == null) continue;
          result.totpSetup.replace(valueDes);
          break;
        case r'backupCodes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.backupCodes.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AuthResultResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AuthResultResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


