//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'document_sequence_response.g.dart';

/// DocumentSequenceResponse
///
/// Properties:
/// * [id] 
/// * [documentType] 
/// * [prefix] 
/// * [formatPattern] 
/// * [resetPeriod] 
/// * [padding] 
/// * [isBranchSpecific] 
/// * [isActive] 
/// * [samplePreview] 
/// * [rowVersion] 
@BuiltValue()
abstract class DocumentSequenceResponse implements Built<DocumentSequenceResponse, DocumentSequenceResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'documentType')
  String? get documentType;

  @BuiltValueField(wireName: r'prefix')
  String? get prefix;

  @BuiltValueField(wireName: r'formatPattern')
  String? get formatPattern;

  @BuiltValueField(wireName: r'resetPeriod')
  String? get resetPeriod;

  @BuiltValueField(wireName: r'padding')
  int? get padding;

  @BuiltValueField(wireName: r'isBranchSpecific')
  bool? get isBranchSpecific;

  @BuiltValueField(wireName: r'isActive')
  bool? get isActive;

  @BuiltValueField(wireName: r'samplePreview')
  String? get samplePreview;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  DocumentSequenceResponse._();

  factory DocumentSequenceResponse([void updates(DocumentSequenceResponseBuilder b)]) = _$DocumentSequenceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DocumentSequenceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DocumentSequenceResponse> get serializer => _$DocumentSequenceResponseSerializer();
}

class _$DocumentSequenceResponseSerializer implements PrimitiveSerializer<DocumentSequenceResponse> {
  @override
  final Iterable<Type> types = const [DocumentSequenceResponse, _$DocumentSequenceResponse];

  @override
  final String wireName = r'DocumentSequenceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DocumentSequenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.documentType != null) {
      yield r'documentType';
      yield serializers.serialize(
        object.documentType,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    if (object.resetPeriod != null) {
      yield r'resetPeriod';
      yield serializers.serialize(
        object.resetPeriod,
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
    if (object.isBranchSpecific != null) {
      yield r'isBranchSpecific';
      yield serializers.serialize(
        object.isBranchSpecific,
        specifiedType: const FullType(bool),
      );
    }
    if (object.isActive != null) {
      yield r'isActive';
      yield serializers.serialize(
        object.isActive,
        specifiedType: const FullType(bool),
      );
    }
    if (object.samplePreview != null) {
      yield r'samplePreview';
      yield serializers.serialize(
        object.samplePreview,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    DocumentSequenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DocumentSequenceResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'documentType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.documentType = valueDes;
          break;
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
        case r'resetPeriod':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resetPeriod = valueDes;
          break;
        case r'padding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.padding = valueDes;
          break;
        case r'isBranchSpecific':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isBranchSpecific = valueDes;
          break;
        case r'isActive':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isActive = valueDes;
          break;
        case r'samplePreview':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.samplePreview = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  DocumentSequenceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DocumentSequenceResponseBuilder();
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


