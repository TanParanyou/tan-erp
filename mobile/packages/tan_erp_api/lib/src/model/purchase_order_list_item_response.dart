//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_order_list_item_response.g.dart';

/// PurchaseOrderListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [status] 
/// * [totalAmount] 
/// * [supplierCode] 
/// * [supplierNameTh] 
/// * [projectCode] 
/// * [expectedDeliveryDate] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class PurchaseOrderListItemResponse implements Built<PurchaseOrderListItemResponse, PurchaseOrderListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'totalAmount')
  double? get totalAmount;

  @BuiltValueField(wireName: r'supplierCode')
  String? get supplierCode;

  @BuiltValueField(wireName: r'supplierNameTh')
  String? get supplierNameTh;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'expectedDeliveryDate')
  Date? get expectedDeliveryDate;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  PurchaseOrderListItemResponse._();

  factory PurchaseOrderListItemResponse([void updates(PurchaseOrderListItemResponseBuilder b)]) = _$PurchaseOrderListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseOrderListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseOrderListItemResponse> get serializer => _$PurchaseOrderListItemResponseSerializer();
}

class _$PurchaseOrderListItemResponseSerializer implements PrimitiveSerializer<PurchaseOrderListItemResponse> {
  @override
  final Iterable<Type> types = const [PurchaseOrderListItemResponse, _$PurchaseOrderListItemResponse];

  @override
  final String wireName = r'PurchaseOrderListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseOrderListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.totalAmount != null) {
      yield r'totalAmount';
      yield serializers.serialize(
        object.totalAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.supplierCode != null) {
      yield r'supplierCode';
      yield serializers.serialize(
        object.supplierCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.supplierNameTh != null) {
      yield r'supplierNameTh';
      yield serializers.serialize(
        object.supplierNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expectedDeliveryDate != null) {
      yield r'expectedDeliveryDate';
      yield serializers.serialize(
        object.expectedDeliveryDate,
        specifiedType: const FullType.nullable(Date),
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
    PurchaseOrderListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseOrderListItemResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'totalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalAmount = valueDes;
          break;
        case r'supplierCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierCode = valueDes;
          break;
        case r'supplierNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierNameTh = valueDes;
          break;
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'expectedDeliveryDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.expectedDeliveryDate = valueDes;
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
  PurchaseOrderListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseOrderListItemResponseBuilder();
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


