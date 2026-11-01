//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/procurement_person_response.dart';
import 'package:tan_erp_api/src/model/supplier_return_line_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_return_response.g.dart';

/// SupplierReturnResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [goodsReceiptId] 
/// * [goodsReceiptNumber] 
/// * [returnedAtUtc] 
/// * [reason] 
/// * [totalReturnAmount] 
/// * [stockDocumentNumber] 
/// * [returnedBy] 
/// * [lines] 
@BuiltValue()
abstract class SupplierReturnResponse implements Built<SupplierReturnResponse, SupplierReturnResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'goodsReceiptId')
  String? get goodsReceiptId;

  @BuiltValueField(wireName: r'goodsReceiptNumber')
  String? get goodsReceiptNumber;

  @BuiltValueField(wireName: r'returnedAtUtc')
  DateTime? get returnedAtUtc;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'totalReturnAmount')
  double? get totalReturnAmount;

  @BuiltValueField(wireName: r'stockDocumentNumber')
  String? get stockDocumentNumber;

  @BuiltValueField(wireName: r'returnedBy')
  ProcurementPersonResponse? get returnedBy;

  @BuiltValueField(wireName: r'lines')
  BuiltList<SupplierReturnLineResponse>? get lines;

  SupplierReturnResponse._();

  factory SupplierReturnResponse([void updates(SupplierReturnResponseBuilder b)]) = _$SupplierReturnResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierReturnResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierReturnResponse> get serializer => _$SupplierReturnResponseSerializer();
}

class _$SupplierReturnResponseSerializer implements PrimitiveSerializer<SupplierReturnResponse> {
  @override
  final Iterable<Type> types = const [SupplierReturnResponse, _$SupplierReturnResponse];

  @override
  final String wireName = r'SupplierReturnResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierReturnResponse object, {
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
    if (object.goodsReceiptId != null) {
      yield r'goodsReceiptId';
      yield serializers.serialize(
        object.goodsReceiptId,
        specifiedType: const FullType(String),
      );
    }
    if (object.goodsReceiptNumber != null) {
      yield r'goodsReceiptNumber';
      yield serializers.serialize(
        object.goodsReceiptNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.returnedAtUtc != null) {
      yield r'returnedAtUtc';
      yield serializers.serialize(
        object.returnedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.totalReturnAmount != null) {
      yield r'totalReturnAmount';
      yield serializers.serialize(
        object.totalReturnAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.stockDocumentNumber != null) {
      yield r'stockDocumentNumber';
      yield serializers.serialize(
        object.stockDocumentNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.returnedBy != null) {
      yield r'returnedBy';
      yield serializers.serialize(
        object.returnedBy,
        specifiedType: const FullType(ProcurementPersonResponse),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierReturnLineResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierReturnResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierReturnResponseBuilder result,
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
        case r'goodsReceiptId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.goodsReceiptId = valueDes;
          break;
        case r'goodsReceiptNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.goodsReceiptNumber = valueDes;
          break;
        case r'returnedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.returnedAtUtc = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'totalReturnAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalReturnAmount = valueDes;
          break;
        case r'stockDocumentNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stockDocumentNumber = valueDes;
          break;
        case r'returnedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPersonResponse),
          ) as ProcurementPersonResponse?;
          if (valueDes == null) continue;
          result.returnedBy.replace(valueDes);
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierReturnLineResponse)]),
          ) as BuiltList<SupplierReturnLineResponse>?;
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
  SupplierReturnResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierReturnResponseBuilder();
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


