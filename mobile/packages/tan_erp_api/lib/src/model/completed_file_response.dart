//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'completed_file_response.g.dart';

/// CompletedFileResponse
///
/// Properties:
/// * [fileId] 
/// * [filename] 
/// * [mediaType] 
/// * [fileSizeBytes] 
/// * [servingUrl] 
@BuiltValue()
abstract class CompletedFileResponse implements Built<CompletedFileResponse, CompletedFileResponseBuilder> {
  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'filename')
  String? get filename;

  @BuiltValueField(wireName: r'mediaType')
  String? get mediaType;

  @BuiltValueField(wireName: r'fileSizeBytes')
  int? get fileSizeBytes;

  @BuiltValueField(wireName: r'servingUrl')
  String? get servingUrl;

  CompletedFileResponse._();

  factory CompletedFileResponse([void updates(CompletedFileResponseBuilder b)]) = _$CompletedFileResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CompletedFileResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CompletedFileResponse> get serializer => _$CompletedFileResponseSerializer();
}

class _$CompletedFileResponseSerializer implements PrimitiveSerializer<CompletedFileResponse> {
  @override
  final Iterable<Type> types = const [CompletedFileResponse, _$CompletedFileResponse];

  @override
  final String wireName = r'CompletedFileResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CompletedFileResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
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
    if (object.servingUrl != null) {
      yield r'servingUrl';
      yield serializers.serialize(
        object.servingUrl,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CompletedFileResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CompletedFileResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileId = valueDes;
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
        case r'servingUrl':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.servingUrl = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CompletedFileResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CompletedFileResponseBuilder();
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


