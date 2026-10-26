//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'preview_item_import_request.g.dart';

/// PreviewItemImportRequest
///
/// Properties:
/// * [content] 
/// * [kind] 
/// * [mode] 
@BuiltValue()
abstract class PreviewItemImportRequest implements Built<PreviewItemImportRequest, PreviewItemImportRequestBuilder> {
  @BuiltValueField(wireName: r'content')
  String? get content;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'mode')
  String? get mode;

  PreviewItemImportRequest._();

  factory PreviewItemImportRequest([void updates(PreviewItemImportRequestBuilder b)]) = _$PreviewItemImportRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PreviewItemImportRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PreviewItemImportRequest> get serializer => _$PreviewItemImportRequestSerializer();
}

class _$PreviewItemImportRequestSerializer implements PrimitiveSerializer<PreviewItemImportRequest> {
  @override
  final Iterable<Type> types = const [PreviewItemImportRequest, _$PreviewItemImportRequest];

  @override
  final String wireName = r'PreviewItemImportRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PreviewItemImportRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.content != null) {
      yield r'content';
      yield serializers.serialize(
        object.content,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.mode != null) {
      yield r'mode';
      yield serializers.serialize(
        object.mode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PreviewItemImportRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PreviewItemImportRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'content':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.content = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'mode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.mode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PreviewItemImportRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PreviewItemImportRequestBuilder();
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


