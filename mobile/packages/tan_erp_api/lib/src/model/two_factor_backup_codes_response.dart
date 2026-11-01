//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'two_factor_backup_codes_response.g.dart';

/// TwoFactorBackupCodesResponse
///
/// Properties:
/// * [backupCodes] 
@BuiltValue()
abstract class TwoFactorBackupCodesResponse implements Built<TwoFactorBackupCodesResponse, TwoFactorBackupCodesResponseBuilder> {
  @BuiltValueField(wireName: r'backupCodes')
  BuiltList<String>? get backupCodes;

  TwoFactorBackupCodesResponse._();

  factory TwoFactorBackupCodesResponse([void updates(TwoFactorBackupCodesResponseBuilder b)]) = _$TwoFactorBackupCodesResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TwoFactorBackupCodesResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TwoFactorBackupCodesResponse> get serializer => _$TwoFactorBackupCodesResponseSerializer();
}

class _$TwoFactorBackupCodesResponseSerializer implements PrimitiveSerializer<TwoFactorBackupCodesResponse> {
  @override
  final Iterable<Type> types = const [TwoFactorBackupCodesResponse, _$TwoFactorBackupCodesResponse];

  @override
  final String wireName = r'TwoFactorBackupCodesResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TwoFactorBackupCodesResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    TwoFactorBackupCodesResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TwoFactorBackupCodesResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  TwoFactorBackupCodesResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TwoFactorBackupCodesResponseBuilder();
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


