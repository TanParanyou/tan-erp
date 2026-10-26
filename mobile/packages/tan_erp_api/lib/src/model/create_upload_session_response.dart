//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/upload_slot_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_upload_session_response.g.dart';

/// CreateUploadSessionResponse
///
/// Properties:
/// * [sessionId] 
/// * [expiresAtUtc] 
/// * [slots] 
@BuiltValue()
abstract class CreateUploadSessionResponse implements Built<CreateUploadSessionResponse, CreateUploadSessionResponseBuilder> {
  @BuiltValueField(wireName: r'sessionId')
  String? get sessionId;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  @BuiltValueField(wireName: r'slots')
  BuiltList<UploadSlotResponse>? get slots;

  CreateUploadSessionResponse._();

  factory CreateUploadSessionResponse([void updates(CreateUploadSessionResponseBuilder b)]) = _$CreateUploadSessionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateUploadSessionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateUploadSessionResponse> get serializer => _$CreateUploadSessionResponseSerializer();
}

class _$CreateUploadSessionResponseSerializer implements PrimitiveSerializer<CreateUploadSessionResponse> {
  @override
  final Iterable<Type> types = const [CreateUploadSessionResponse, _$CreateUploadSessionResponse];

  @override
  final String wireName = r'CreateUploadSessionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateUploadSessionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sessionId != null) {
      yield r'sessionId';
      yield serializers.serialize(
        object.sessionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.expiresAtUtc != null) {
      yield r'expiresAtUtc';
      yield serializers.serialize(
        object.expiresAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.slots != null) {
      yield r'slots';
      yield serializers.serialize(
        object.slots,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UploadSlotResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateUploadSessionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateUploadSessionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sessionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sessionId = valueDes;
          break;
        case r'expiresAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.expiresAtUtc = valueDes;
          break;
        case r'slots':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UploadSlotResponse)]),
          ) as BuiltList<UploadSlotResponse>?;
          if (valueDes == null) continue;
          result.slots.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateUploadSessionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateUploadSessionResponseBuilder();
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


