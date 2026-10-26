//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'preview_document_sequence_request.g.dart';

/// PreviewDocumentSequenceRequest
///
/// Properties:
/// * [prefix] 
/// * [formatPattern] 
/// * [branchCode] 
/// * [padding] 
/// * [sampleSequence] 
@BuiltValue()
abstract class PreviewDocumentSequenceRequest implements Built<PreviewDocumentSequenceRequest, PreviewDocumentSequenceRequestBuilder> {
  @BuiltValueField(wireName: r'prefix')
  String? get prefix;

  @BuiltValueField(wireName: r'formatPattern')
  String? get formatPattern;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'padding')
  int? get padding;

  @BuiltValueField(wireName: r'sampleSequence')
  int? get sampleSequence;

  PreviewDocumentSequenceRequest._();

  factory PreviewDocumentSequenceRequest([void updates(PreviewDocumentSequenceRequestBuilder b)]) = _$PreviewDocumentSequenceRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PreviewDocumentSequenceRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PreviewDocumentSequenceRequest> get serializer => _$PreviewDocumentSequenceRequestSerializer();
}

class _$PreviewDocumentSequenceRequestSerializer implements PrimitiveSerializer<PreviewDocumentSequenceRequest> {
  @override
  final Iterable<Type> types = const [PreviewDocumentSequenceRequest, _$PreviewDocumentSequenceRequest];

  @override
  final String wireName = r'PreviewDocumentSequenceRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PreviewDocumentSequenceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.prefix != null) {
      yield r'prefix';
      yield serializers.serialize(
        object.prefix,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.formatPattern != null) {
      yield r'formatPattern';
      yield serializers.serialize(
        object.formatPattern,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchCode != null) {
      yield r'branchCode';
      yield serializers.serialize(
        object.branchCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.padding != null) {
      yield r'padding';
      yield serializers.serialize(
        object.padding,
        specifiedType: const FullType(int),
      );
    }
    if (object.sampleSequence != null) {
      yield r'sampleSequence';
      yield serializers.serialize(
        object.sampleSequence,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PreviewDocumentSequenceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PreviewDocumentSequenceRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'prefix':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.prefix = valueDes;
          break;
        case r'formatPattern':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.formatPattern = valueDes;
          break;
        case r'branchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchCode = valueDes;
          break;
        case r'padding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.padding = valueDes;
          break;
        case r'sampleSequence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sampleSequence = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PreviewDocumentSequenceRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PreviewDocumentSequenceRequestBuilder();
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


