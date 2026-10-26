//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'catalog_attribute_facet_response.g.dart';

/// CatalogAttributeFacetResponse
///
/// Properties:
/// * [key] 
/// * [value] 
/// * [count] 
@BuiltValue()
abstract class CatalogAttributeFacetResponse implements Built<CatalogAttributeFacetResponse, CatalogAttributeFacetResponseBuilder> {
  @BuiltValueField(wireName: r'key')
  String? get key;

  @BuiltValueField(wireName: r'value')
  String? get value;

  @BuiltValueField(wireName: r'count')
  int? get count;

  CatalogAttributeFacetResponse._();

  factory CatalogAttributeFacetResponse([void updates(CatalogAttributeFacetResponseBuilder b)]) = _$CatalogAttributeFacetResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CatalogAttributeFacetResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CatalogAttributeFacetResponse> get serializer => _$CatalogAttributeFacetResponseSerializer();
}

class _$CatalogAttributeFacetResponseSerializer implements PrimitiveSerializer<CatalogAttributeFacetResponse> {
  @override
  final Iterable<Type> types = const [CatalogAttributeFacetResponse, _$CatalogAttributeFacetResponse];

  @override
  final String wireName = r'CatalogAttributeFacetResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CatalogAttributeFacetResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.key != null) {
      yield r'key';
      yield serializers.serialize(
        object.key,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.value != null) {
      yield r'value';
      yield serializers.serialize(
        object.value,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.count != null) {
      yield r'count';
      yield serializers.serialize(
        object.count,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CatalogAttributeFacetResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CatalogAttributeFacetResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'key':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.key = valueDes;
          break;
        case r'value':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.value = valueDes;
          break;
        case r'count':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.count = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CatalogAttributeFacetResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CatalogAttributeFacetResponseBuilder();
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


