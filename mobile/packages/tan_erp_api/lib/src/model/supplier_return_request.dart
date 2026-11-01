//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/supplier_return_line_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_return_request.g.dart';

/// SupplierReturnRequest
///
/// Properties:
/// * [goodsReceiptId] 
/// * [returnedAtUtc] 
/// * [reason] 
/// * [lines] 
@BuiltValue()
abstract class SupplierReturnRequest implements Built<SupplierReturnRequest, SupplierReturnRequestBuilder> {
  @BuiltValueField(wireName: r'goodsReceiptId')
  String? get goodsReceiptId;

  @BuiltValueField(wireName: r'returnedAtUtc')
  DateTime? get returnedAtUtc;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'lines')
  BuiltList<SupplierReturnLineRequest>? get lines;

  SupplierReturnRequest._();

  factory SupplierReturnRequest([void updates(SupplierReturnRequestBuilder b)]) = _$SupplierReturnRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierReturnRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierReturnRequest> get serializer => _$SupplierReturnRequestSerializer();
}

class _$SupplierReturnRequestSerializer implements PrimitiveSerializer<SupplierReturnRequest> {
  @override
  final Iterable<Type> types = const [SupplierReturnRequest, _$SupplierReturnRequest];

  @override
  final String wireName = r'SupplierReturnRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierReturnRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.goodsReceiptId != null) {
      yield r'goodsReceiptId';
      yield serializers.serialize(
        object.goodsReceiptId,
        specifiedType: const FullType(String),
      );
    }
    if (object.returnedAtUtc != null) {
      yield r'returnedAtUtc';
      yield serializers.serialize(
        object.returnedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierReturnLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierReturnRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierReturnRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'goodsReceiptId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.goodsReceiptId = valueDes;
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
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierReturnLineRequest)]),
          ) as BuiltList<SupplierReturnLineRequest>?;
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
  SupplierReturnRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierReturnRequestBuilder();
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


