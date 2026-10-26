//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/production_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_list_item_response.g.dart';

/// BomListItemResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [item] 
/// * [approvedRevisionNo] 
/// * [latestRevisionNo] 
/// * [latestStatus] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class BomListItemResponse implements Built<BomListItemResponse, BomListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'item')
  ProductionItemResponse? get item;

  @BuiltValueField(wireName: r'approvedRevisionNo')
  int? get approvedRevisionNo;

  @BuiltValueField(wireName: r'latestRevisionNo')
  int? get latestRevisionNo;

  @BuiltValueField(wireName: r'latestStatus')
  String? get latestStatus;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  BomListItemResponse._();

  factory BomListItemResponse([void updates(BomListItemResponseBuilder b)]) = _$BomListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomListItemResponse> get serializer => _$BomListItemResponseSerializer();
}

class _$BomListItemResponseSerializer implements PrimitiveSerializer<BomListItemResponse> {
  @override
  final Iterable<Type> types = const [BomListItemResponse, _$BomListItemResponse];

  @override
  final String wireName = r'BomListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(ProductionItemResponse),
      );
    }
    if (object.approvedRevisionNo != null) {
      yield r'approvedRevisionNo';
      yield serializers.serialize(
        object.approvedRevisionNo,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.latestRevisionNo != null) {
      yield r'latestRevisionNo';
      yield serializers.serialize(
        object.latestRevisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.latestStatus != null) {
      yield r'latestStatus';
      yield serializers.serialize(
        object.latestStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomListItemResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionItemResponse),
          ) as ProductionItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'approvedRevisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.approvedRevisionNo = valueDes;
          break;
        case r'latestRevisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.latestRevisionNo = valueDes;
          break;
        case r'latestStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.latestStatus = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomListItemResponseBuilder();
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


