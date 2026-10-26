//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/completed_file_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'complete_upload_session_response.g.dart';

/// CompleteUploadSessionResponse
///
/// Properties:
/// * [sessionId] 
/// * [files] 
@BuiltValue()
abstract class CompleteUploadSessionResponse implements Built<CompleteUploadSessionResponse, CompleteUploadSessionResponseBuilder> {
  @BuiltValueField(wireName: r'sessionId')
  String? get sessionId;

  @BuiltValueField(wireName: r'files')
  BuiltList<CompletedFileResponse>? get files;

  CompleteUploadSessionResponse._();

  factory CompleteUploadSessionResponse([void updates(CompleteUploadSessionResponseBuilder b)]) = _$CompleteUploadSessionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CompleteUploadSessionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CompleteUploadSessionResponse> get serializer => _$CompleteUploadSessionResponseSerializer();
}

class _$CompleteUploadSessionResponseSerializer implements PrimitiveSerializer<CompleteUploadSessionResponse> {
  @override
  final Iterable<Type> types = const [CompleteUploadSessionResponse, _$CompleteUploadSessionResponse];

  @override
  final String wireName = r'CompleteUploadSessionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CompleteUploadSessionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sessionId != null) {
      yield r'sessionId';
      yield serializers.serialize(
        object.sessionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.files != null) {
      yield r'files';
      yield serializers.serialize(
        object.files,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CompletedFileResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CompleteUploadSessionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CompleteUploadSessionResponseBuilder result,
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
        case r'files':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CompletedFileResponse)]),
          ) as BuiltList<CompletedFileResponse>?;
          if (valueDes == null) continue;
          result.files.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CompleteUploadSessionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CompleteUploadSessionResponseBuilder();
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


