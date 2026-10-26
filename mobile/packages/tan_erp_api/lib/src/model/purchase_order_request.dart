//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/purchase_order_line_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_order_request.g.dart';

/// PurchaseOrderRequest
///
/// Properties:
/// * [supplierId] 
/// * [projectId] 
/// * [expectedDeliveryDate] 
/// * [note] 
/// * [lines] 
@BuiltValue()
abstract class PurchaseOrderRequest implements Built<PurchaseOrderRequest, PurchaseOrderRequestBuilder> {
  @BuiltValueField(wireName: r'supplierId')
  String? get supplierId;

  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'expectedDeliveryDate')
  Date? get expectedDeliveryDate;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'lines')
  BuiltList<PurchaseOrderLineRequest>? get lines;

  PurchaseOrderRequest._();

  factory PurchaseOrderRequest([void updates(PurchaseOrderRequestBuilder b)]) = _$PurchaseOrderRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseOrderRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseOrderRequest> get serializer => _$PurchaseOrderRequestSerializer();
}

class _$PurchaseOrderRequestSerializer implements PrimitiveSerializer<PurchaseOrderRequest> {
  @override
  final Iterable<Type> types = const [PurchaseOrderRequest, _$PurchaseOrderRequest];

  @override
  final String wireName = r'PurchaseOrderRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseOrderRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.supplierId != null) {
      yield r'supplierId';
      yield serializers.serialize(
        object.supplierId,
        specifiedType: const FullType(String),
      );
    }
    if (object.projectId != null) {
      yield r'projectId';
      yield serializers.serialize(
        object.projectId,
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
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseOrderLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseOrderRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseOrderRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'supplierId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierId = valueDes;
          break;
        case r'projectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectId = valueDes;
          break;
        case r'expectedDeliveryDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.expectedDeliveryDate = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseOrderLineRequest)]),
          ) as BuiltList<PurchaseOrderLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseOrderRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseOrderRequestBuilder();
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


