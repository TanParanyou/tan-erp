//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'regenerate_backup_codes_request.g.dart';

/// RegenerateBackupCodesRequest
///
/// Properties:
/// * [code] 
@BuiltValue()
abstract class RegenerateBackupCodesRequest implements Built<RegenerateBackupCodesRequest, RegenerateBackupCodesRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  RegenerateBackupCodesRequest._();

  factory RegenerateBackupCodesRequest([void updates(RegenerateBackupCodesRequestBuilder b)]) = _$RegenerateBackupCodesRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RegenerateBackupCodesRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RegenerateBackupCodesRequest> get serializer => _$RegenerateBackupCodesRequestSerializer();
}

class _$RegenerateBackupCodesRequestSerializer implements PrimitiveSerializer<RegenerateBackupCodesRequest> {
  @override
  final Iterable<Type> types = const [RegenerateBackupCodesRequest, _$RegenerateBackupCodesRequest];

  @override
  final String wireName = r'RegenerateBackupCodesRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RegenerateBackupCodesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RegenerateBackupCodesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RegenerateBackupCodesRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RegenerateBackupCodesRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RegenerateBackupCodesRequestBuilder();
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


