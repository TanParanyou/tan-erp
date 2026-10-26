//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/permission_catalog_entry_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'permission_catalog_response.g.dart';

/// PermissionCatalogResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class PermissionCatalogResponse implements Built<PermissionCatalogResponse, PermissionCatalogResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<PermissionCatalogEntryResponse>? get items;

  PermissionCatalogResponse._();

  factory PermissionCatalogResponse([void updates(PermissionCatalogResponseBuilder b)]) = _$PermissionCatalogResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PermissionCatalogResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PermissionCatalogResponse> get serializer => _$PermissionCatalogResponseSerializer();
}

class _$PermissionCatalogResponseSerializer implements PrimitiveSerializer<PermissionCatalogResponse> {
  @override
  final Iterable<Type> types = const [PermissionCatalogResponse, _$PermissionCatalogResponse];

  @override
  final String wireName = r'PermissionCatalogResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PermissionCatalogResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PermissionCatalogEntryResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PermissionCatalogResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PermissionCatalogResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PermissionCatalogEntryResponse)]),
          ) as BuiltList<PermissionCatalogEntryResponse>?;
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
  PermissionCatalogResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PermissionCatalogResponseBuilder();
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


