//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'catalog_page_info_response.g.dart';

/// CatalogPageInfoResponse
///
/// Properties:
/// * [nextCursor] 
/// * [hasNextPage] 
@BuiltValue()
abstract class CatalogPageInfoResponse implements Built<CatalogPageInfoResponse, CatalogPageInfoResponseBuilder> {
  @BuiltValueField(wireName: r'nextCursor')
  String? get nextCursor;

  @BuiltValueField(wireName: r'hasNextPage')
  bool? get hasNextPage;

  CatalogPageInfoResponse._();

  factory CatalogPageInfoResponse([void updates(CatalogPageInfoResponseBuilder b)]) = _$CatalogPageInfoResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CatalogPageInfoResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CatalogPageInfoResponse> get serializer => _$CatalogPageInfoResponseSerializer();
}

class _$CatalogPageInfoResponseSerializer implements PrimitiveSerializer<CatalogPageInfoResponse> {
  @override
  final Iterable<Type> types = const [CatalogPageInfoResponse, _$CatalogPageInfoResponse];

  @override
  final String wireName = r'CatalogPageInfoResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CatalogPageInfoResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.nextCursor != null) {
      yield r'nextCursor';
      yield serializers.serialize(
        object.nextCursor,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.hasNextPage != null) {
      yield r'hasNextPage';
      yield serializers.serialize(
        object.hasNextPage,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CatalogPageInfoResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CatalogPageInfoResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'nextCursor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nextCursor = valueDes;
          break;
        case r'hasNextPage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasNextPage = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CatalogPageInfoResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CatalogPageInfoResponseBuilder();
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


