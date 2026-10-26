//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'upload_slot_response.g.dart';

/// UploadSlotResponse
///
/// Properties:
/// * [slotId] 
/// * [filename] 
/// * [mediaType] 
/// * [fileSizeBytes] 
@BuiltValue()
abstract class UploadSlotResponse implements Built<UploadSlotResponse, UploadSlotResponseBuilder> {
  @BuiltValueField(wireName: r'slotId')
  String? get slotId;

  @BuiltValueField(wireName: r'filename')
  String? get filename;

  @BuiltValueField(wireName: r'mediaType')
  String? get mediaType;

  @BuiltValueField(wireName: r'fileSizeBytes')
  int? get fileSizeBytes;

  UploadSlotResponse._();

  factory UploadSlotResponse([void updates(UploadSlotResponseBuilder b)]) = _$UploadSlotResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UploadSlotResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UploadSlotResponse> get serializer => _$UploadSlotResponseSerializer();
}

class _$UploadSlotResponseSerializer implements PrimitiveSerializer<UploadSlotResponse> {
  @override
  final Iterable<Type> types = const [UploadSlotResponse, _$UploadSlotResponse];

  @override
  final String wireName = r'UploadSlotResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UploadSlotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.slotId != null) {
      yield r'slotId';
      yield serializers.serialize(
        object.slotId,
        specifiedType: const FullType(String),
      );
    }
    if (object.filename != null) {
      yield r'filename';
      yield serializers.serialize(
        object.filename,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.mediaType != null) {
      yield r'mediaType';
      yield serializers.serialize(
        object.mediaType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.fileSizeBytes != null) {
      yield r'fileSizeBytes';
      yield serializers.serialize(
        object.fileSizeBytes,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UploadSlotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UploadSlotResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'slotId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.slotId = valueDes;
          break;
        case r'filename':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.filename = valueDes;
          break;
        case r'mediaType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.mediaType = valueDes;
          break;
        case r'fileSizeBytes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.fileSizeBytes = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UploadSlotResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UploadSlotResponseBuilder();
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


