//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'catalog_facet_value_response.g.dart';

/// CatalogFacetValueResponse
///
/// Properties:
/// * [value] 
/// * [count] 
@BuiltValue()
abstract class CatalogFacetValueResponse implements Built<CatalogFacetValueResponse, CatalogFacetValueResponseBuilder> {
  @BuiltValueField(wireName: r'value')
  String? get value;

  @BuiltValueField(wireName: r'count')
  int? get count;

  CatalogFacetValueResponse._();

  factory CatalogFacetValueResponse([void updates(CatalogFacetValueResponseBuilder b)]) = _$CatalogFacetValueResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CatalogFacetValueResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CatalogFacetValueResponse> get serializer => _$CatalogFacetValueResponseSerializer();
}

class _$CatalogFacetValueResponseSerializer implements PrimitiveSerializer<CatalogFacetValueResponse> {
  @override
  final Iterable<Type> types = const [CatalogFacetValueResponse, _$CatalogFacetValueResponse];

  @override
  final String wireName = r'CatalogFacetValueResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CatalogFacetValueResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    CatalogFacetValueResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CatalogFacetValueResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  CatalogFacetValueResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CatalogFacetValueResponseBuilder();
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


