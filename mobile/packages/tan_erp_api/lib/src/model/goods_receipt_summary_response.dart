//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/procurement_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'goods_receipt_summary_response.g.dart';

/// GoodsReceiptSummaryResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [receivedAtUtc] 
/// * [note] 
/// * [receivedBy] 
/// * [lineCount] 
/// * [totalQuantity] 
/// * [stockDocumentNumber] 
@BuiltValue()
abstract class GoodsReceiptSummaryResponse implements Built<GoodsReceiptSummaryResponse, GoodsReceiptSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'receivedAtUtc')
  DateTime? get receivedAtUtc;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'receivedBy')
  ProcurementPersonResponse? get receivedBy;

  @BuiltValueField(wireName: r'lineCount')
  int? get lineCount;

  @BuiltValueField(wireName: r'totalQuantity')
  double? get totalQuantity;

  @BuiltValueField(wireName: r'stockDocumentNumber')
  String? get stockDocumentNumber;

  GoodsReceiptSummaryResponse._();

  factory GoodsReceiptSummaryResponse([void updates(GoodsReceiptSummaryResponseBuilder b)]) = _$GoodsReceiptSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(GoodsReceiptSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<GoodsReceiptSummaryResponse> get serializer => _$GoodsReceiptSummaryResponseSerializer();
}

class _$GoodsReceiptSummaryResponseSerializer implements PrimitiveSerializer<GoodsReceiptSummaryResponse> {
  @override
  final Iterable<Type> types = const [GoodsReceiptSummaryResponse, _$GoodsReceiptSummaryResponse];

  @override
  final String wireName = r'GoodsReceiptSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    GoodsReceiptSummaryResponse object, {
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
    if (object.receivedAtUtc != null) {
      yield r'receivedAtUtc';
      yield serializers.serialize(
        object.receivedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.receivedBy != null) {
      yield r'receivedBy';
      yield serializers.serialize(
        object.receivedBy,
        specifiedType: const FullType(ProcurementPersonResponse),
      );
    }
    if (object.lineCount != null) {
      yield r'lineCount';
      yield serializers.serialize(
        object.lineCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.totalQuantity != null) {
      yield r'totalQuantity';
      yield serializers.serialize(
        object.totalQuantity,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    GoodsReceiptSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required GoodsReceiptSummaryResponseBuilder result,
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
        case r'receivedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.receivedAtUtc = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'receivedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPersonResponse),
          ) as ProcurementPersonResponse?;
          if (valueDes == null) continue;
          result.receivedBy.replace(valueDes);
          break;
        case r'lineCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.lineCount = valueDes;
          break;
        case r'totalQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalQuantity = valueDes;
          break;
        case r'stockDocumentNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stockDocumentNumber = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  GoodsReceiptSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = GoodsReceiptSummaryResponseBuilder();
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


