//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/stock_line_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'issue_stock_request.g.dart';

/// IssueStockRequest
///
/// Properties:
/// * [warehouseId] 
/// * [projectId] 
/// * [reason] 
/// * [lines] 
@BuiltValue()
abstract class IssueStockRequest implements Built<IssueStockRequest, IssueStockRequestBuilder> {
  @BuiltValueField(wireName: r'warehouseId')
  String? get warehouseId;

  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'lines')
  BuiltList<StockLineRequest>? get lines;

  IssueStockRequest._();

  factory IssueStockRequest([void updates(IssueStockRequestBuilder b)]) = _$IssueStockRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(IssueStockRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<IssueStockRequest> get serializer => _$IssueStockRequestSerializer();
}

class _$IssueStockRequestSerializer implements PrimitiveSerializer<IssueStockRequest> {
  @override
  final Iterable<Type> types = const [IssueStockRequest, _$IssueStockRequest];

  @override
  final String wireName = r'IssueStockRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    IssueStockRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.warehouseId != null) {
      yield r'warehouseId';
      yield serializers.serialize(
        object.warehouseId,
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
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    IssueStockRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required IssueStockRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'warehouseId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.warehouseId = valueDes;
          break;
        case r'projectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectId = valueDes;
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
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockLineRequest)]),
          ) as BuiltList<StockLineRequest>?;
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
  IssueStockRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = IssueStockRequestBuilder();
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


