//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'preview_document_sequence_response.g.dart';

/// PreviewDocumentSequenceResponse
///
/// Properties:
/// * [preview] 
@BuiltValue()
abstract class PreviewDocumentSequenceResponse implements Built<PreviewDocumentSequenceResponse, PreviewDocumentSequenceResponseBuilder> {
  @BuiltValueField(wireName: r'preview')
  String? get preview;

  PreviewDocumentSequenceResponse._();

  factory PreviewDocumentSequenceResponse([void updates(PreviewDocumentSequenceResponseBuilder b)]) = _$PreviewDocumentSequenceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PreviewDocumentSequenceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PreviewDocumentSequenceResponse> get serializer => _$PreviewDocumentSequenceResponseSerializer();
}

class _$PreviewDocumentSequenceResponseSerializer implements PrimitiveSerializer<PreviewDocumentSequenceResponse> {
  @override
  final Iterable<Type> types = const [PreviewDocumentSequenceResponse, _$PreviewDocumentSequenceResponse];

  @override
  final String wireName = r'PreviewDocumentSequenceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PreviewDocumentSequenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.preview != null) {
      yield r'preview';
      yield serializers.serialize(
        object.preview,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PreviewDocumentSequenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PreviewDocumentSequenceResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'preview':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.preview = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PreviewDocumentSequenceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PreviewDocumentSequenceResponseBuilder();
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


