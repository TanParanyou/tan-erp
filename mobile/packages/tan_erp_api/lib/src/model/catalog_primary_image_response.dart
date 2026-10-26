//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'catalog_primary_image_response.g.dart';

/// CatalogPrimaryImageResponse
///
/// Properties:
/// * [fileId] 
/// * [altText] 
@BuiltValue()
abstract class CatalogPrimaryImageResponse implements Built<CatalogPrimaryImageResponse, CatalogPrimaryImageResponseBuilder> {
  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'altText')
  LocalizedTextResponse? get altText;

  CatalogPrimaryImageResponse._();

  factory CatalogPrimaryImageResponse([void updates(CatalogPrimaryImageResponseBuilder b)]) = _$CatalogPrimaryImageResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CatalogPrimaryImageResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CatalogPrimaryImageResponse> get serializer => _$CatalogPrimaryImageResponseSerializer();
}

class _$CatalogPrimaryImageResponseSerializer implements PrimitiveSerializer<CatalogPrimaryImageResponse> {
  @override
  final Iterable<Type> types = const [CatalogPrimaryImageResponse, _$CatalogPrimaryImageResponse];

  @override
  final String wireName = r'CatalogPrimaryImageResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CatalogPrimaryImageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.altText != null) {
      yield r'altText';
      yield serializers.serialize(
        object.altText,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CatalogPrimaryImageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CatalogPrimaryImageResponseBuilder result,
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
        case r'altText':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.altText.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CatalogPrimaryImageResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CatalogPrimaryImageResponseBuilder();
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


