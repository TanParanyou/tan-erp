//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'file_slot_request.g.dart';

/// FileSlotRequest
///
/// Properties:
/// * [filename] 
/// * [mediaType] 
/// * [fileSizeBytes] 
@BuiltValue()
abstract class FileSlotRequest implements Built<FileSlotRequest, FileSlotRequestBuilder> {
  @BuiltValueField(wireName: r'filename')
  String? get filename;

  @BuiltValueField(wireName: r'mediaType')
  String? get mediaType;

  @BuiltValueField(wireName: r'fileSizeBytes')
  int? get fileSizeBytes;

  FileSlotRequest._();

  factory FileSlotRequest([void updates(FileSlotRequestBuilder b)]) = _$FileSlotRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(FileSlotRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<FileSlotRequest> get serializer => _$FileSlotRequestSerializer();
}

class _$FileSlotRequestSerializer implements PrimitiveSerializer<FileSlotRequest> {
  @override
  final Iterable<Type> types = const [FileSlotRequest, _$FileSlotRequest];

  @override
  final String wireName = r'FileSlotRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    FileSlotRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    FileSlotRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required FileSlotRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  FileSlotRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = FileSlotRequestBuilder();
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


