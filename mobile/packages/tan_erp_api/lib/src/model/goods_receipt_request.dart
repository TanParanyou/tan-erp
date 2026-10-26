//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/goods_receipt_line_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'goods_receipt_request.g.dart';

/// GoodsReceiptRequest
///
/// Properties:
/// * [receivedAtUtc] 
/// * [note] 
/// * [lines] 
@BuiltValue()
abstract class GoodsReceiptRequest implements Built<GoodsReceiptRequest, GoodsReceiptRequestBuilder> {
  @BuiltValueField(wireName: r'receivedAtUtc')
  DateTime? get receivedAtUtc;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'lines')
  BuiltList<GoodsReceiptLineRequest>? get lines;

  GoodsReceiptRequest._();

  factory GoodsReceiptRequest([void updates(GoodsReceiptRequestBuilder b)]) = _$GoodsReceiptRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(GoodsReceiptRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<GoodsReceiptRequest> get serializer => _$GoodsReceiptRequestSerializer();
}

class _$GoodsReceiptRequestSerializer implements PrimitiveSerializer<GoodsReceiptRequest> {
  @override
  final Iterable<Type> types = const [GoodsReceiptRequest, _$GoodsReceiptRequest];

  @override
  final String wireName = r'GoodsReceiptRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    GoodsReceiptRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.receivedAtUtc != null) {
      yield r'receivedAtUtc';
      yield serializers.serialize(
        object.receivedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
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
        specifiedType: const FullType.nullable(BuiltList, [FullType(GoodsReceiptLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    GoodsReceiptRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required GoodsReceiptRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(GoodsReceiptLineRequest)]),
          ) as BuiltList<GoodsReceiptLineRequest>?;
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
  GoodsReceiptRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = GoodsReceiptRequestBuilder();
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


