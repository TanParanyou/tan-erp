//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'my_security_status_response.g.dart';

/// MySecurityStatusResponse
///
/// Properties:
/// * [hasPassword] 
/// * [hasGoogleLinked] 
/// * [isTotpEnabled] 
/// * [requireTwoFactor] 
/// * [remainingBackupCodesCount] 
/// * [totpEnabledAtUtc] 
/// * [passwordChangedAtUtc] 
@BuiltValue()
abstract class MySecurityStatusResponse implements Built<MySecurityStatusResponse, MySecurityStatusResponseBuilder> {
  @BuiltValueField(wireName: r'hasPassword')
  bool? get hasPassword;

  @BuiltValueField(wireName: r'hasGoogleLinked')
  bool? get hasGoogleLinked;

  @BuiltValueField(wireName: r'isTotpEnabled')
  bool? get isTotpEnabled;

  @BuiltValueField(wireName: r'requireTwoFactor')
  bool? get requireTwoFactor;

  @BuiltValueField(wireName: r'remainingBackupCodesCount')
  int? get remainingBackupCodesCount;

  @BuiltValueField(wireName: r'totpEnabledAtUtc')
  DateTime? get totpEnabledAtUtc;

  @BuiltValueField(wireName: r'passwordChangedAtUtc')
  DateTime? get passwordChangedAtUtc;

  MySecurityStatusResponse._();

  factory MySecurityStatusResponse([void updates(MySecurityStatusResponseBuilder b)]) = _$MySecurityStatusResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MySecurityStatusResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MySecurityStatusResponse> get serializer => _$MySecurityStatusResponseSerializer();
}

class _$MySecurityStatusResponseSerializer implements PrimitiveSerializer<MySecurityStatusResponse> {
  @override
  final Iterable<Type> types = const [MySecurityStatusResponse, _$MySecurityStatusResponse];

  @override
  final String wireName = r'MySecurityStatusResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MySecurityStatusResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.hasPassword != null) {
      yield r'hasPassword';
      yield serializers.serialize(
        object.hasPassword,
        specifiedType: const FullType(bool),
      );
    }
    if (object.hasGoogleLinked != null) {
      yield r'hasGoogleLinked';
      yield serializers.serialize(
        object.hasGoogleLinked,
        specifiedType: const FullType(bool),
      );
    }
    if (object.isTotpEnabled != null) {
      yield r'isTotpEnabled';
      yield serializers.serialize(
        object.isTotpEnabled,
        specifiedType: const FullType(bool),
      );
    }
    if (object.requireTwoFactor != null) {
      yield r'requireTwoFactor';
      yield serializers.serialize(
        object.requireTwoFactor,
        specifiedType: const FullType(bool),
      );
    }
    if (object.remainingBackupCodesCount != null) {
      yield r'remainingBackupCodesCount';
      yield serializers.serialize(
        object.remainingBackupCodesCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.totpEnabledAtUtc != null) {
      yield r'totpEnabledAtUtc';
      yield serializers.serialize(
        object.totpEnabledAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.passwordChangedAtUtc != null) {
      yield r'passwordChangedAtUtc';
      yield serializers.serialize(
        object.passwordChangedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MySecurityStatusResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MySecurityStatusResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'hasPassword':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasPassword = valueDes;
          break;
        case r'hasGoogleLinked':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasGoogleLinked = valueDes;
          break;
        case r'isTotpEnabled':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isTotpEnabled = valueDes;
          break;
        case r'requireTwoFactor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.requireTwoFactor = valueDes;
          break;
        case r'remainingBackupCodesCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.remainingBackupCodesCount = valueDes;
          break;
        case r'totpEnabledAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.totpEnabledAtUtc = valueDes;
          break;
        case r'passwordChangedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.passwordChangedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MySecurityStatusResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MySecurityStatusResponseBuilder();
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


