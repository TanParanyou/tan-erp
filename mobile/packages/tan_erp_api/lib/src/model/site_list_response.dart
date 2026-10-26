//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/site_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'site_list_response.g.dart';

/// SiteListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class SiteListResponse implements Built<SiteListResponse, SiteListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<SiteResponse>? get items;

  SiteListResponse._();

  factory SiteListResponse([void updates(SiteListResponseBuilder b)]) = _$SiteListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SiteListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SiteListResponse> get serializer => _$SiteListResponseSerializer();
}

class _$SiteListResponseSerializer implements PrimitiveSerializer<SiteListResponse> {
  @override
  final Iterable<Type> types = const [SiteListResponse, _$SiteListResponse];

  @override
  final String wireName = r'SiteListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SiteListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SiteResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SiteListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SiteListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SiteResponse)]),
          ) as BuiltList<SiteResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SiteListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SiteListResponseBuilder();
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


