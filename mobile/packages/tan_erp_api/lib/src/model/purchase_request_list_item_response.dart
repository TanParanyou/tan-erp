//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_request_list_item_response.g.dart';

/// PurchaseRequestListItemResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [status] 
/// * [totalEstimatedAmount] 
/// * [projectCode] 
/// * [requiredByDate] 
/// * [convertedPurchaseOrderId] 
/// * [convertedPurchaseOrderNumber] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class PurchaseRequestListItemResponse implements Built<PurchaseRequestListItemResponse, PurchaseRequestListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'totalEstimatedAmount')
  double? get totalEstimatedAmount;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'requiredByDate')
  Date? get requiredByDate;

  @BuiltValueField(wireName: r'convertedPurchaseOrderId')
  String? get convertedPurchaseOrderId;

  @BuiltValueField(wireName: r'convertedPurchaseOrderNumber')
  String? get convertedPurchaseOrderNumber;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  PurchaseRequestListItemResponse._();

  factory PurchaseRequestListItemResponse([void updates(PurchaseRequestListItemResponseBuilder b)]) = _$PurchaseRequestListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseRequestListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseRequestListItemResponse> get serializer => _$PurchaseRequestListItemResponseSerializer();
}

class _$PurchaseRequestListItemResponseSerializer implements PrimitiveSerializer<PurchaseRequestListItemResponse> {
  @override
  final Iterable<Type> types = const [PurchaseRequestListItemResponse, _$PurchaseRequestListItemResponse];

  @override
  final String wireName = r'PurchaseRequestListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseRequestListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
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
    if (object.totalEstimatedAmount != null) {
      yield r'totalEstimatedAmount';
      yield serializers.serialize(
        object.totalEstimatedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.requiredByDate != null) {
      yield r'requiredByDate';
      yield serializers.serialize(
        object.requiredByDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.convertedPurchaseOrderId != null) {
      yield r'convertedPurchaseOrderId';
      yield serializers.serialize(
        object.convertedPurchaseOrderId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.convertedPurchaseOrderNumber != null) {
      yield r'convertedPurchaseOrderNumber';
      yield serializers.serialize(
        object.convertedPurchaseOrderNumber,
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
    PurchaseRequestListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseRequestListItemResponseBuilder result,
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
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
        case r'totalEstimatedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalEstimatedAmount = valueDes;
          break;
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'requiredByDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.requiredByDate = valueDes;
          break;
        case r'convertedPurchaseOrderId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.convertedPurchaseOrderId = valueDes;
          break;
        case r'convertedPurchaseOrderNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.convertedPurchaseOrderNumber = valueDes;
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
  PurchaseRequestListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseRequestListItemResponseBuilder();
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


